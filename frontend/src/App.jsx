import { useState } from 'react'

function App() {
  const [sourceCode, setSourceCode] = useState('');
  const [snippetId, setSnippetId] = useState('');

  const sendToServer = async () => {
    let response = await fetch('/api/code/run', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        sourceCode: sourceCode,
        snippetId: snippetId ? parseInt(snippetId, 10) : null
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

  const handleSnippetChange = async (e) => {
    const selectedId = e.target.value;
    setSnippetId(selectedId);

    if (!selectedId) {
      setSourceCode('');
      return;
    }

    try {
      const response = await fetch(`/api/code/snippet/${selectedId}`);
      if (!response.ok) {
        throw new Error('Failed to fetch snippet from server');
      }
      
      const data = await response.json();
      setSourceCode(data.sourceCode);
      
    } catch (error) {
      console.error("Error fetching snippet:", error);
    }
  }

  return (
    <>
      <select value={snippetId} onChange={handleSnippetChange}>
        <option value="">Select a snippet...</option>
        <option value="1">Snippet 1</option>
      </select>
      
      <br />

      <textarea 
        id="source-code" 
        value={sourceCode} 
        onChange={(e) => setSourceCode(e.target.value)}
        rows="10"
        cols="50"
      ></textarea>
      
      <br />

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
