$(document).ready(function() {
    toggleChat();
    loadMessages();

    function loadMessages()
    {
        $.get('/api/message/get', function(data) {
            $('#chatMessages').empty();
            data.forEach(function(msg) {
                const messageClass = msg.isAdminReply ? 'admin-message' : 'user-message';
                $('#chatMessages').append('<div class="${messageClass}">${msg.content}</div>');
            });
        });
    }
});
</ script >

< style >
    .user - message {
background: #007bff;
        color: white;
padding: 5px 10px;
margin: 10px;
    border - radius: 8px;
    align - self: flex - end;
}
    .admin - message {
background: #28a745;
        color: 5px;
padding:
    white;
margin: 10px;
    border - radius: 8px;
    align - self: flex - start;
}
</ style >