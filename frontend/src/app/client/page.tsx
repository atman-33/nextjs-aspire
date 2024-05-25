'use client'

import { useEffect, useState } from 'react';

const getData = async () => {
  // NOTE:  WebApi プロジェクトの launchSettings.json ファイル > profiles > http > applicationUrl
  // "applicationUrl": "http://localhost:5291",
  const weatherData: Response = await fetch('http://localhost:5291/api/weatherforecast', { cache: 'no-cache' })

  if(!weatherData.ok) {
    throw new Error('Failed to fetch data.')
  }

  const data = await weatherData.json();
  return data;
}

const ClientPage = () => {
  // console.log('running in client')

  const [data, setData] =  useState([]);

  useEffect(() => {
    getData().then((data) => setData(data));
  }, [])

  return (
    <main>
      {JSON.stringify(data)}
    </main>
  )
}

export default ClientPage