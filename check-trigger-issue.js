const sql = require('mssql');

const config = {
  server: 'localhost',
  port: 1433,
  user: 'sa',
  password: 'YourStrong@Passw0rd',
  database: 'TRUNGTAMTHETHAO',
  options: {
    trustServerCertificate: true,
    encrypt: false,
    connectionTimeout: 30000,
    requestTimeout: 30000
  }
};

async function checkStatus() {
  try {
    await sql.connect(config);
    
    // Check actual status values in SAN table
    const result = await sql.query`SELECT DISTINCT TinhTrang FROM SAN`;
    
    console.log('Current SAN.TinhTrang values:');
    result.recordset.forEach(row => {
      console.log(`  - "${row.TinhTrang}"`);
    });

    // Check trigger definition
    const triggerDef = await sql.query`
      SELECT OBJECT_DEFINITION(OBJECT_ID('trg_ChuyenTrangThaiSan')) AS TriggerCode
    `;
    
    if (triggerDef.recordset[0].TriggerCode) {
      console.log('\nTrigger content (first 800 chars):');
      console.log(triggerDef.recordset[0].TriggerCode.substring(0, 800));
    }
    
    sql.close();
  } catch (err) {
    console.error('Error:', err.message);
    process.exit(1);
  }
}

checkStatus();
