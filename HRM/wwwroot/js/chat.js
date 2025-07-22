class ChatManager {
    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/chathub")
            .withAutomaticReconnect()
            .build();

        this.receiverId = "";
        this.currentUserId = document.querySelector('meta[name="user-id"]').content; // Store user ID in a meta tag
        this.messagesContainer = document.getElementById("messages");
        this.messageInput = document.getElementById("messageInput");
        this.chatBox = document.getElementById("chatBox");
        this.chatWith = document.getElementById("chatWith");

        this.initializeEventListeners();
        this.startConnection();
    }

    initializeEventListeners() {
        // Handle incoming messages
        this.connection.on("ReceiveMessage", (senderId, message) => {
            this.appendMessage(senderId, message);
        });

        // Send message on Enter key
        this.messageInput.addEventListener("keypress", async (e) => {
            if (e.key === "Enter") {
                await this.sendMessage();
            }
        });
    }

    async startConnection() {
        try {
            await this.connection.start();
            console.log("SignalR Connected");
        } catch (err) {
            console.error("SignalR Connection Error:", err);
        }
    }

    async startChat(id, username) {
        this.receiverId = id;
        this.chatBox.style.display = "block";
        this.chatWith.textContent = `Chat with ${username}`;
        this.messagesContainer.innerHTML = "";

        // Load previous messages
        try {
            const response = await fetch(`/Chat/GetMessages?receiverId=${id}`);
            const messages = await response.json();
            messages.forEach((msg) => this.appendMessage(msg.senderId, msg.message));
        } catch (err) {
            console.error("Error loading messages:", err);
        }
    }

    async sendMessage() {
        const message = this.messageInput.value.trim();
        if (!message || !this.receiverId) return;

        try {
            await this.connection.invoke("SendMessage", this.receiverId, message);
            this.messageInput.value = "";
        } catch (err) {
            console.error("Error sending message:", err);
        }
    }

    appendMessage(senderId, message) {
        const isCurrentUser = senderId === this.currentUserId;
        const messageClass = isCurrentUser ? "message-self" : "message-other";
        const messageElement = document.createElement("div");
        messageElement.className = messageClass;
        messageElement.innerHTML = `<b>${senderId}</b>: ${message}`;
        this.messagesContainer.appendChild(messageElement);
        this.messagesContainer.scrollTop = this.messagesContainer.scrollHeight;
    }
}

// Initialize chat
document.addEventListener("DOMContentLoaded", () => {
    window.chatManager = new ChatManager();
});

// Expose startChat and sendMessage globally for HTML onclick
window.startChat = (id, username) => window.chatManager.startChat(id, username);
window.sendMessage = () => window.chatManager.sendMessage();