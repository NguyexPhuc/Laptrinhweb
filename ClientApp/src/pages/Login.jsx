import React, { useState } from 'react'
import axios from 'axios'
import { useNavigate } from 'react-router-dom'

export default function Login(){
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [mode, setMode] = useState('cookie') // 'cookie' or 'jwt'
  const nav = useNavigate()

  const submit = async (e)=>{
    e.preventDefault()
    try{
      if (mode === 'cookie'){
        await axios.post('/api/auth/login-cookie', { username, password, remember: false }, { withCredentials: true })
        nav('/')
      } else {
        const r = await axios.post('/api/auth/token', { username, password })
        localStorage.setItem('jwt', r.data.access_token)
        nav('/')
      }
    }catch(err){
      alert('Login failed')
    }
  }

  return (
    <div>
      <h1>Login</h1>
      <label>Mode</label>
      <select value={mode} onChange={e=>setMode(e.target.value)}>
        <option value="cookie">Cookie-based</option>
        <option value="jwt">JWT</option>
      </select>
      <form onSubmit={submit}>
        <label>Username</label>
        <input value={username} onChange={e=>setUsername(e.target.value)} />
        <label>Password</label>
        <input type="password" value={password} onChange={e=>setPassword(e.target.value)} />
        <button>Login</button>
      </form>
    </div>
  )
}