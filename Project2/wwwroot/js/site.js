var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

// Start the connection
connection.start().catch(function (err) {
    return console.error(err.toString());
});

//document.getElementById("deleteButton").addEventListener("click", function (event) {
    
//    connection.invoke("Reload").catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//}, { once: true });

connection.on("Update", function () {
    location.reload();
});