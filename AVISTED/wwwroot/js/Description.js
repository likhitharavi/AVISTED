function openDialog(id,param) {
    
    debugger;
    var btn, model;
    if (param === '0') {
        // Get the modal
         modal = document.getElementById(id + '+myModal');
      
        // Get the button that opens the modal
         btn = document.getElementById(id + "+myBtn");
        
        
    } else {
        // Get the modal
         modal = document.getElementById(id + '+myModalparam');

        // Get the button that opens the modal
         btn = document.getElementById(id + "+mybtnparam");

    }
    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];
    // When the user clicks on the button, open the modal 
    
        modal.style.display = "block";
    
    // When the user clicks on <span> (x), close the modal
    span.onclick = function () {
      modal.style.display = "none";
    }

    
    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
         if (event.target == modal) {
             modal.style.display = "none";
         }
     } 
}

function closeDialog(id, param) {
    var model;
    if (param === '0') {
         modal = document.getElementById(id + '+myModal');
        modal.style.display = "none";
    } else {
         modal = document.getElementById(id + '+myModalparam');
        modal.style.display = "none";
    }

}

function openUpload() {
    debugger;
    var modal = document.getElementById("modal1");

    // Get the button that opens the modal
    var btn = document.getElementById("upload");
    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];
    // When the user clicks on the button, open the modal 

    modal.style.display = "block";

    // When the user clicks on <span> (x), close the modal
    span.onclick = function () {
        modal.style.display = "none";
    }


    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    }
}

function closeUpload()
{
    
        var modal = document.getElementById(id + '+modal1');
        modal.style.display = "none";
    
}