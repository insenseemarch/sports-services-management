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

async function testPaymentFlow() {
    const pool = new sql.ConnectionPool(config);
    
    try {
        await pool.connect();
        console.log('🔄 Testing thanh toán flow...\n');
        
        // Lấy 1 booking chưa thanh toán
        const bookings = await pool.request().query(`
            SELECT TOP 1 p.MaDatSan, p.MaKH, p.NgayDat, p.GioBatDau, p.GioKetThuc, p.TrangThai
            FROM PHIEUDATSAN p
            ORDER BY p.MaDatSan DESC
        `);
        
        if (bookings.recordset.length === 0) {
            console.log('❌ Không có booking nào');
            return;
        }
        
        const booking = bookings.recordset[0];
        const maDatSan = booking.MaDatSan;
        
        console.log(`📌 Test với Booking MaDatSan: ${maDatSan}`);
        console.log(`   KH: ${booking.MaKH}, Ngày: ${booking.NgayDat}, Giờ: ${booking.GioBatDau}-${booking.GioKetThuc}`);
        console.log(`   Trạng thái trước: ${booking.TrangThai}\n`);
        
        // Kiểm tra trạng thái sân trước thanh toán
        const courtBefore = await pool.request().query(`
            SELECT s.MaSan, s.TinhTrang
            FROM SAN s JOIN DATSAN d ON s.MaSan = d.MaSan
            WHERE d.MaDatSan = @MaDatSan
        `, { MaDatSan: maDatSan });
        
        if (courtBefore.recordset.length > 0) {
            console.log(`🏟️  Sân trước TT: ${courtBefore.recordset[0].MaSan} = "${courtBefore.recordset[0].TinhTrang}"`);
        }
        
        // Gọi sp_ThanhToanOnline
        console.log('\n⏳ Gọi sp_ThanhToanOnline...');
        try {
            await pool.request()
                .input('MaDatSan', sql.BigInt, maDatSan)
                .input('NguoiLap', sql.VarChar, 'NV001')
                .input('HinhThucTT', sql.NVarChar, 'QR')
                .input('MaUD', sql.VarChar, null)
                .execute('sp_ThanhToanOnline');
            
            console.log('✅ sp_ThanhToanOnline thành công!');
        } catch (err) {
            console.log(`❌ sp_ThanhToanOnline lỗi: ${err.message}`);
            await pool.close();
            process.exit(1);
        }
        
        // Kiểm tra trạng thái sau thanh toán
        console.log('\n🔍 Kiểm tra sau thanh toán...');
        
        const bookingAfter = await pool.request().query(`
            SELECT TrangThai FROM PHIEUDATSAN WHERE MaDatSan = @MaDatSan
        `, { MaDatSan: maDatSan });
        
        console.log(`   ✓ PHIEUDATSAN.TrangThai = "${bookingAfter.recordset[0].TrangThai}"`);
        
        const courtAfter = await pool.request().query(`
            SELECT s.MaSan, s.TinhTrang
            FROM SAN s JOIN DATSAN d ON s.MaSan = d.MaSan
            WHERE d.MaDatSan = @MaDatSan
        `, { MaDatSan: maDatSan });
        
        if (courtAfter.recordset.length > 0) {
            console.log(`   ✓ SAN.TinhTrang = "${courtAfter.recordset[0].TinhTrang}"`);
        }
        
        const invoice = await pool.request().query(`
            SELECT MaHD, ThanhTien, HinhThucTT FROM HOADON WHERE MaPhieu = @MaDatSan
        `, { MaDatSan: maDatSan });
        
        if (invoice.recordset.length > 0) {
            const hd = invoice.recordset[0];
            console.log(`   ✓ HOADON: MaHD=${hd.MaHD}, ThanhTien=${hd.ThanhTien}, HinhThucTT=${hd.HinhThucTT}`);
        }
        
        console.log('\n✅ ✅ ✅ THANH TOÁN HOÀN TOÀN THÀNH CÔNG ✅ ✅ ✅');
        
        await pool.close();
    } catch (err) {
        console.error('❌ Lỗi:', err.message);
        process.exit(1);
    }
}

testPaymentFlow();
