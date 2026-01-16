const sql = require('mssql');
const fs = require('fs');
const path = require('path');

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

async function resetDatabase() {
    const pool = new sql.ConnectionPool(config);
    
    try {
        console.log('🔄 Kết nối database...');
        await pool.connect();
        
        // Drop and recreate SP từ file gốc
        console.log('📝 Áp dụng Function+SP+Trigger.sql gốc...');
        const sqlPath = path.join(__dirname, 'database/TRUNGTAMTHETHAO/Function+SP+Trigger.sql');
        let sqlContent = fs.readFileSync(sqlPath, 'utf8');
        
        // Split by GO statements
        const batches = sqlContent
            .split(/^GO\s*$/gim)
            .map(b => b.trim())
            .filter(b => b.length > 0);
        
        for (let i = 0; i < batches.length; i++) {
            try {
                await pool.request().batch(batches[i]);
                process.stdout.write(`\r⏳ Batch ${i + 1}/${batches.length}`);
            } catch (err) {
                if (!err.message.includes('already an open')) {
                    console.error(`\n❌ Batch ${i + 1} failed:`, err.message);
                }
            }
        }
        
        console.log('\n✅ Database reset thành công!');
        
        // Verify sp_ThanhToanOnline có UPDATE SAN
        const result = await pool.request()
            .query(`SELECT OBJECT_DEFINITION(OBJECT_ID('sp_ThanhToanOnline')) as ProcCode`);
        
        if (result.recordset[0]) {
            const code = result.recordset[0].ProcCode;
            if (code.includes('UPDATE SAN SET TinhTrang')) {
                console.log('✅ sp_ThanhToanOnline: UPDATE SAN có mặt ✓');
            } else {
                console.log('❌ sp_ThanhToanOnline: UPDATE SAN KHÔNG có!');
            }
        }
        
        await pool.close();
    } catch (err) {
        console.error('❌ Lỗi:', err.message);
        process.exit(1);
    }
}

resetDatabase();
