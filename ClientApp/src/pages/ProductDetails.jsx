import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import axios from 'axios'

export default function ProductDetails(){
  const { id } = useParams()
  const [p, setP] = useState(null)
  useEffect(()=>{ axios.get(`/api/products/${id}`).then(r => setP(r.data)).catch(console.error) },[id])
  if (!p) return <div>Loading...</div>
  return (
    <div>
      <h1>{p.productName}</h1>
      <img src={p.imageUrl} alt="" />
      <p>{p.description}</p>
      <p>Giá: {p.basePrice}</p>
    </div>
  )
}