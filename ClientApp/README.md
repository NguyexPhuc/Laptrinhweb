ClientApp (React + Node)

Scripts
- npm install
- npm run dev (development with Vite and proxy to ASP.NET backend at https://localhost:5001)
- npm run build (create production build into dist)
- npm run start-server (start Express server that serves dist and proxies /api)

Notes
- By default vite proxy is configured to https://localhost:5001 for /api calls. Adjust `vite.config.js` for backend URL.