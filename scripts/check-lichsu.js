const sql = require('mssql');

const config = {
    server: 'localhost',
    port: 1433,
    database: 'TRUNGTAMTHETHAO',
    user: 'sa',
    password: 'YourStrong@Passw0rd',
    options: { trustServerCertificate: true, encrypt: false }
};

async function checkLichSu() {
    const pool = new sql.ConnectionPool(config);
    try {
        await pool.connect();
        console.log('🔍 Kiểm tra dữ liệu cho trang lịch sử...\n');
        
        // Check PHIEUDATSAN data
        const result = await pool.request().query(`
            SELECT p.MaDatSan, d.MaSan, p.NgayDat, p.GioBatDau, p.GioKetThuc, p.MaKH, p.TrangThai
            FROM PHIEUDATSAN p
            JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
            ORDER BY p.MaDatSan
        `);
        
        console.log(`Tổng booking: ${result.recordset.length}\n`);
        
        result.recordset.forEach(row => {
            console.log(`MaDatSan: ${row.MaDatSan}`);
            console.log(`  KH: ${row.MaKH}, Sân: ${row.MaSan}`);
            console.log(`  Ngày: ${row.NgayDat}`);
            console.log(`  Giờ: ${row.GioBatDau} - ${row.GioKetThuc}`);
            console.log(`  Trạng thái: ${row.TrangThai}`);
            
            // Check if GioBatDau is null or weird
            if (!row.GioBatDau) {
                console.log(`  ⚠️  GioBatDau is NULL!`);
            }
            if (!row.GioKetThuc) {
                console.log(`  ⚠️  GioKetThuc is NULL!`);
            }
            console.log();
        });
        
        // Test f_TinhTienSan for each booking
        console.log('\n🧪 Test f_TinhTienSan:\n');
        for (let i = 0; i < Math.min(3, result.recordset.length); i++) {
            const maDatSan = result.recordset[i].MaDatSan;
            try {
                const priceResult = await pool.request()
                    .input('MaDatSan', sql.BigInt, maDatSan)
                    .query(`SELECT dbo.f_TinhTienSan(@MaDatSan) as TienSan`);
                
                console.log(`MaDatSan ${maDatSan}: ${priceResult.recordset[0].TienSan} đ`);
            } catch (err) {
                console.log(`MaDatSan ${maDatSan}: ❌ Error - ${err.message}`);
            }
        }
        
        await pool.close();
    } catch (err) {
        console.error('Error:', err.message);
    }
}

checkLichSu();
