import { useState } from 'react'

function App() {
  const [sourceCode, setSourceCode] = useState('');

  const sendToServer = async () => {
    let response = await fetch('/api/code/run', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        sourceCode: sourceCode
      }) 
    });

    if (!response.ok) {
      const errorDetails = await response.text(); 
      console.error("Server Error:", errorDetails);
      throw new Error('Network response was not ok');
  }

    let data = await response.json();
    return data;
  }

  return (
    <>
      <textarea id="source-code" onChange={(e) => setSourceCode(e.target.value)}></textarea>
      <button onClick={async () => {
        try {
          let result = await sendToServer();
          console.log(result);
        } catch (error) {
          console.error('Error sending to server:', error);
        }
      }}>Run</button>
    </>
  )
}

export default App
