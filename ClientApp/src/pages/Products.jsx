import React, { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import axios from 'axios'

export default function Products(){
  const [products, setProducts] = useState([])
  useEffect(()=>{ axios.get('/api/products').then(r => setProducts(r.data)).catch(console.error) },[])
  return (
    <div>
      <h1>Products</h1>
      <div className="grid">
        {products.map(p => (
          <div className="card" key={p.productId}>
            <img src={p.imageUrl} alt="" />
            <h3><Link to={`/products/${p.productId}`}>{p.productName}</Link></h3>
            <p>{p.basePrice} USD</p>
          </div>
        ))}
      </div>
    </div>
  )
}