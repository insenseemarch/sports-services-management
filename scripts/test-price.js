const sql = require('mssql');

const config = {
    server: 'localhost',
    port: 1433,
    database: 'TRUNGTAMTHETHAO',
    user: 'sa',
    password: 'YourStrong@Passw0rd',
    options: { trustServerCertificate: true, encrypt: false }
};

async function test() {
    const pool = new sql.ConnectionPool(config);
    try {
        await pool.connect();
        
        console.log('🔍 KHUNGGIO data:\n');
        const kg = await pool.request()
            .query(`SELECT TOP 5 MaKG, GioBatDau, GioKetThuc, MaLS, GiaApDung FROM KHUNGGIO ORDER BY MaKG`);
        
        kg.recordset.forEach(r => {
            console.log(`${r.GioBatDau}-${r.GioKetThuc} | MaLS=${r.MaLS} | Giá=${r.GiaApDung}`);
        });
        
        console.log('\n🔍 Booking #23 details:\n');
        const b = await pool.request()
            .query(`
                SELECT p.MaDatSan, p.NgayDat, p.GioBatDau, p.GioKetThuc, s.MaLS, ls.TenLS, s.MaSan
                FROM PHIEUDATSAN p
                JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                JOIN SAN s ON d.MaSan = s.MaSan
                JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                WHERE p.MaDatSan = 23
            `);
        
        if (b.recordset.length > 0) {
            const row = b.recordset[0];
            console.log(`Booking: ${row.MaDatSan}`);
            console.log(`Ngày: ${row.NgayDat}`);
            console.log(`Giờ: ${row.GioBatDau} - ${row.GioKetThuc}`);
            console.log(`Sân: ${row.MaSan} | Loại: ${row.TenLS} (${row.MaLS})`);
        }
        
        console.log('\n\n🔍 f_TinhTienSan(23):\n');
        const price = await pool.request()
            .input('MaDatSan', sql.BigInt, 23)
            .query(`SELECT dbo.f_TinhTienSan(@MaDatSan) as TienSan`);
        
        console.log(`Result: ${price.recordset[0].TienSan} đ`);
        
        await pool.close();
    } catch (err) {
        console.error('Error:', err.message);
    }
}

test();
