const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 8080;

// Serve static build
app.use(express.static(path.join(__dirname, '..', 'dist')));

// Proxy API requests to ASP.NET backend
app.use('/api', createProxyMiddleware({
  target: process.env.API_URL || 'https://localhost:5001',
  changeOrigin: true,
  secure: false
}));

// Fallback to index.html
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, '..', 'dist', 'index.html'));
});

app.listen(PORT, () => {
  console.log(`Client server started on port ${PORT}`);
});