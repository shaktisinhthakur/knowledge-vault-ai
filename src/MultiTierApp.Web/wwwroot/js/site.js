document.addEventListener('DOMContentLoaded', () => {
    // Stat counter animation on Dashboard page
    const counters = document.querySelectorAll('.stat-number');
    counters.forEach(counter => {
        const updateCount = () => {
            const target = +counter.getAttribute('data-target');
            const count = +counter.innerText;
            const speed = 200; // lower is faster
            const inc = Math.max(1, Math.ceil(target / speed));

            if (count < target) {
                counter.innerText = count + inc;
                setTimeout(updateCount, 1);
            } else {
                counter.innerText = target;
            }
        };
        updateCount();
    });

    // RAG Chat Feature Integration
    const chatForm = document.getElementById('chatForm');
    const userInput = document.getElementById('userInput');
    const chatMessages = document.getElementById('chatMessages');
    const sourcesContainer = document.getElementById('sourcesContainer');
    const clearChatBtn = document.getElementById('clearChatBtn');

    if (chatForm && userInput && chatMessages) {
        chatForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const text = userInput.value.trim();
            if (!text) return;

            // Clear input field
            userInput.value = '';

            // 1. Append User Message to UI
            appendMessage(text, 'user', 'bi-person-fill');

            // 2. Append Assistant Typing Indicator
            const typingIndicatorId = appendTypingIndicator();

            // Auto-scroll
            scrollToBottom();

            try {
                // 3. Send query via AJAX fetch call
                const response = await fetch('/Rag/Ask', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ question: text })
                });

                // Remove typing indicator
                removeTypingIndicator(typingIndicatorId);

                if (!response.ok) {
                    throw new Error(`Server returned status code ${response.status}`);
                }

                const data = await response.json();

                if (data.success) {
                    // 4. Append Assistant Response to UI
                    appendMessage(data.answer, 'assistant', 'bi-stars');
                    
                    // 5. Render retrieved sources
                    renderSources(data.sources);
                } else {
                    appendMessage(data.errorMessage || "Sorry, I couldn't complete that request.", 'assistant', 'bi-stars');
                }
            } catch (error) {
                console.error(error);
                removeTypingIndicator(typingIndicatorId);
                appendMessage("An error occurred. Make sure your application, Qdrant, and Ollama containers are running correctly.", 'assistant', 'bi-stars');
            }

            scrollToBottom();
        });

        // Clear Chat button click handler
        if (clearChatBtn) {
            clearChatBtn.addEventListener('click', () => {
                chatMessages.innerHTML = `
                    <div class="message-wrapper assistant">
                        <div class="message-icon">
                            <i class="bi bi-stars"></i>
                        </div>
                        <div class="message-bubble">
                            Hello! I am your Retrieval-Augmented Generation (RAG) assistant. 
                            Ask me anything about the documents you've uploaded and embedded. 
                            I'll retrieve relevant sections and answer using Groq AI.
                        </div>
                    </div>
                `;
                sourcesContainer.innerHTML = `
                    <div class="text-center text-secondary py-5">
                        <i class="bi bi-search" style="font-size: 2rem;"></i>
                        <p class="mt-3 small">Ask a question to see semantic search results.</p>
                    </div>
                `;
            });
        }
    }

    function appendMessage(text, sender, iconClass) {
        const wrapper = document.createElement('div');
        wrapper.className = `message-wrapper ${sender}`;

        const iconContainer = document.createElement('div');
        iconContainer.className = 'message-icon';
        iconContainer.innerHTML = `<i class="bi ${iconClass}"></i>`;

        const bubble = document.createElement('div');
        bubble.className = 'message-bubble';
        
        // Simple line break support and basic rendering
        bubble.innerHTML = text.replace(/\n/g, '<br/>');

        wrapper.appendChild(iconContainer);
        wrapper.appendChild(bubble);
        chatMessages.appendChild(wrapper);
    }

    function appendTypingIndicator() {
        const id = 'typing_' + Date.now();
        const wrapper = document.createElement('div');
        wrapper.className = 'message-wrapper assistant';
        wrapper.id = id;

        const iconContainer = document.createElement('div');
        iconContainer.className = 'message-icon';
        iconContainer.innerHTML = `<i class="bi bi-stars"></i>`;

        const bubble = document.createElement('div');
        bubble.className = 'message-bubble';
        bubble.innerHTML = `
            <div class="typing-indicator">
                <span class="typing-dot"></span>
                <span class="typing-dot"></span>
                <span class="typing-dot"></span>
            </div>
        `;

        wrapper.appendChild(iconContainer);
        wrapper.appendChild(bubble);
        chatMessages.appendChild(wrapper);
        return id;
    }

    function removeTypingIndicator(id) {
        const indicator = document.getElementById(id);
        if (indicator) {
            indicator.remove();
        }
    }

    function renderSources(sources) {
        if (!sources || sources.length === 0) {
            sourcesContainer.innerHTML = `
                <div class="text-center text-secondary py-5">
                    <i class="bi bi-search" style="font-size: 2rem;"></i>
                    <p class="mt-3 small">No sources matching this question were found.</p>
                </div>
            `;
            return;
        }

        sourcesContainer.innerHTML = '';
        sources.forEach(source => {
            const item = document.createElement('div');
            item.className = 'source-item';
            item.innerHTML = `
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <strong class="text-light small text-truncate" style="max-width: 70%;">${source.documentTitle}</strong>
                    <span class="badge bg-secondary small">Match: ${(source.score * 100).toFixed(0)}%</span>
                </div>
                <p class="text-secondary small mb-0 font-monospace">${source.chunkPreview}</p>
            `;
            sourcesContainer.appendChild(item);
        });
    }

    function scrollToBottom() {
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }
});
