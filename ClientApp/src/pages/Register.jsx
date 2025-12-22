import React, { useState } from 'react'
import axios from 'axios'
import { useNavigate } from 'react-router-dom'

export default function Register(){
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const nav = useNavigate()

  const submit = async e =>{
    e.preventDefault()
    try{
      await axios.post('/api/auth/register', { username, email, password })
      alert('Registered. Please check email to confirm.')
      nav('/login')
    }catch(err){
      alert('Register failed')
    }
  }

  return (
    <div>
      <h1>Register</h1>
      <form onSubmit={submit}>
        <label>Username</label>
        <input value={username} onChange={e=>setUsername(e.target.value)} />
        <label>Email</label>
        <input value={email} onChange={e=>setEmail(e.target.value)} />
        <label>Password</label>
        <input type="password" value={password} onChange={e=>setPassword(e.target.value)} />
        <button>Register</button>
      </form>
    </div>
  )
}