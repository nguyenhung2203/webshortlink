const fetch = require('node-fetch');
async function test() {
  try {
    const res = await fetch('http://localhost:5242/api/public/auth/login', {
      method: 'POST', headers: {'Content-Type': 'application/json'},
      body: JSON.stringify({email:'test_new@example.com', password:'Password123!'})
    });
    const d = await res.json();
    console.log('Login:', res.status, d.accessToken ? 'ok' : d);
    if (!d.accessToken) return;

    const overRes = await fetch('http://localhost:5242/api/user/analytics/overview', {
      headers: {'Authorization': 'Bearer ' + d.accessToken}
    });
    console.log('Overview Status:', overRes.status);
    console.log('Overview Body:', await overRes.text());
  } catch (e) {
    console.log(e.message);
  }
}
test();
