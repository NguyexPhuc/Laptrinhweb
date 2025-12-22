import React, { useEffect, useState } from 'react'
import axios from 'axios'

export default function Home(){
  const [products, setProducts] = useState([])
  useEffect(()=>{
    axios.get('/api/products').then(r => setProducts(r.data)).catch(console.error)
  },[])
  return (
    <div>
      <h1>Sản phẩm nổi bật</h1>
      <div className="grid">
        {products.map(p => (
          <div className="card" key={p.productId}>
            <img src={p.imageUrl} alt="" />
            <h3>{p.productName}</h3>
            <p>{p.basePrice} USD</p>
          </div>
        ))}
      </div>
    </div>
  )
}