const sql = require('mssql');

const config = {
    server: 'localhost',
    port: 1433,
    database: 'TRUNGTAMTHETHAO',
    user: 'sa',
    password: 'YourStrong@Passw0rd',
    options: {
        trustServerCertificate: true,
        encrypt: false,
    }
};

async function checkPricing() {
    const pool = new sql.ConnectionPool(config);
    
    try {
        await pool.connect();
        console.log('🔍 Kiểm tra tính giá sân...\n');
        
        // Check function f_TinhTienSan
        console.log('1️⃣  Hàm f_TinhTienSan:\n');
        const funcResult = await pool.request()
            .query(`SELECT OBJECT_DEFINITION(OBJECT_ID('f_TinhTienSan')) as FuncCode`);
        
        if (funcResult.recordset[0]) {
            const code = funcResult.recordset[0].FuncCode;
            console.log(code);
        }
        
        // Check KHUNGGIO data
        console.log('\n\n2️⃣  Dữ liệu KHUNGGIO:\n');
        const khungGioResult = await pool.request()
            .query(`
                SELECT TOP 10 kg.MaKG, kg.GioBatDau, kg.GioKetThuc, kg.MaLS, kg.Gia, ls.TenLS
                FROM KHUNGGIO kg
                JOIN LOAISAN ls ON kg.MaLS = ls.MaLS
                ORDER BY kg.MaKG
            `);
        
        console.log('KHUNGGIO records:');
        khungGioResult.recordset.forEach(row => {
            console.log(`  MaKG: ${row.MaKG}, ${row.GioBatDau}-${row.GioKetThuc}, ${row.TenLS}, Giá: ${row.Gia}`);
        });
        
        // Test specific case: 18:10 - 19:10 with Bóng rổ
        console.log('\n\n3️⃣  Test case: Thời gian 18:10-19:10 + Bóng rổ:\n');
        
        // First get MaLS for Bóng rổ
        const ropeLS = await pool.request()
            .query(`SELECT MaLS FROM LOAISAN WHERE TenLS LIKE N'%Bóng rổ%'`);
        
        if (ropeLS.recordset.length > 0) {
            const maLS = ropeLS.recordset[0].MaLS;
            console.log(`Bóng rổ MaLS: ${maLS}`);
            
            // Check KHUNGGIO for this time range
            const timeSlot = await pool.request()
                .query(`
                    SELECT MaKG, GioBatDau, GioKetThuc, Gia
                    FROM KHUNGGIO
                    WHERE MaLS = @MaLS AND GioBatDau = '18:10:00' OR GioBatDau = '18:10'
                `, { MaLS: maLS });
            
            console.log(`\nKHUNGGIO cho Bóng rổ vào 18:10:`);
            if (timeSlot.recordset.length > 0) {
                timeSlot.recordset.forEach(row => {
                    console.log(`  ${row.GioBatDau}-${row.GioKetThuc}: Giá = ${row.Gia}`);
                });
            } else {
                console.log('  ❌ KHÔNG TÌM THẤY');
            }
        }
        
        // Test f_TinhTienSan with a booking
        console.log('\n\n4️⃣  Test hàm f_TinhTienSan với booking #23:\n');
        const priceResult = await pool.request()
            .input('MaDatSan', sql.BigInt, 23)
            .query(`SELECT dbo.f_TinhTienSan(@MaDatSan) as TienSan`);
        
        console.log(`f_TinhTienSan(23) = ${priceResult.recordset[0].TienSan}`);
        
        // Get booking details
        const bookingDetail = await pool.request()
            .query(`
                SELECT p.MaDatSan, p.NgayDat, p.GioBatDau, p.GioKetThuc, 
                       s.MaSan, ls.TenLS, ls.MaLS
                FROM PHIEUDATSAN p
                JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                JOIN SAN s ON d.MaSan = s.MaSan
                JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                WHERE p.MaDatSan = 23
            `);
        
        if (bookingDetail.recordset.length > 0) {
            const b = bookingDetail.recordset[0];
            console.log(`\nBooking #23 details:`);
            console.log(`  Ngày: ${b.NgayDat}`);
            console.log(`  Giờ: ${b.GioBatDau} - ${b.GioKetThuc}`);
            console.log(`  Sân: ${b.MaSan} (${b.TenLS})`);
        }
        
        await pool.close();
    } catch (err) {
        console.error('❌ Lỗi:', err.message);
        process.exit(1);
    }
}

checkPricing();
