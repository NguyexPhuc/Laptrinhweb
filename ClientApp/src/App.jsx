import React from 'react'
import { Routes, Route, Link } from 'react-router-dom'
import Home from './pages/Home'
import Products from './pages/Products'
import ProductDetails from './pages/ProductDetails'
import Login from './pages/Login'
import Register from './pages/Register'

export default function App(){
  return (
    <div>
      <nav className="nav">
        <Link to="/">Home</Link> | <Link to="/products">Products</Link> | <Link to="/login">Login</Link>
      </nav>
      <main>
        <Routes>
          <Route path="/" element={<Home/>} />
          <Route path="/products" element={<Products/>} />
          <Route path="/products/:id" element={<ProductDetails/>} />
          <Route path="/login" element={<Login/>} />
          <Route path="/register" element={<Register/>} />
        </Routes>
      </main>
    </div>
  )
}