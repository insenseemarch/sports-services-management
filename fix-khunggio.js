const sql = require('mssql');

const config = {
    server: 'localhost',
    port: 1433,
    database: 'TRUNGTAMTHETHAO',
    user: 'sa',
    password: 'YourStrong@Passw0rd',
    options: { trustServerCertificate: true, encrypt: false }
};

async function fixKhungGio() {
    const pool = new sql.ConnectionPool(config);
    try {
        await pool.connect();
        console.log('🔄 Thêm khung giờ 18:00-22:00 cho Bóng rổ...');
        
        await pool.request().query(`
            INSERT INTO KHUNGGIO (MaKG, MaLS, GioBatDau, GioKetThuc, NgayApDung, GiaApDung)
            VALUES ('KG016', 'LS004', '18:00:00', '22:00:00', '2024-01-01', 200000)
        `);
        
        console.log('✅ Khung giờ đã thêm');
        
        // Test f_TinhTienSan(23)
        console.log('\n🧪 Test f_TinhTienSan(23)...');
        const result = await pool.request()
            .input('MaDatSan', sql.BigInt, 23)
            .query(`SELECT dbo.f_TinhTienSan(@MaDatSan) as TienSan`);
        
        const tienSan = result.recordset[0].TienSan;
        console.log(`Result: ${tienSan} đ`);
        
        if (tienSan > 0) {
            console.log('✅ ✅ ✅ GIÁ TÍNH ĐÚNG RỒI ✅ ✅ ✅');
        } else {
            console.log('⚠️  Vẫn trả về 0');
        }
        
        await pool.close();
    } catch (err) {
        console.error('Error:', err.message);
    }
}

fixKhungGio();
