

//to switch between white star to yellow star uponclick.

let ratingGlobal = 0

    function RateStar(rating) {
        switch (rating) {
            case 1:

                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "hidden";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "hidden";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "hidden";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "hidden";
                ratingGlobal = rating;

                break;

            case 2:

                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "visible";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "hidden";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "hidden";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "hidden";
                ratingGlobal = rating;

                break;

            case 3:
                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "visible";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "visible";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "hidden";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "hidden";
                ratingGlobal = rating;

                break;

            case 4:

                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "visible";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "visible";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "visible";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "hidden";
                ratingGlobal = rating;

                break;

            case 5:

                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "visible";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "visible";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "visible";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "visible";

                ratingGlobal = rating;
                break;
            default:
                break;
        }
}

//must rate the product before submitting
function ValidateReview()
{
        if (ratingGlobal == 0) {
            //show alert to rate the product before submitting
            $('#rateProduct').modal('show');            
            return false;
        }
        else {
            document.getElementById("ratingDummyInput").value = ratingGlobal;
            //show alert to to Thank for reviewing            
            $('#reviewSubmitted').modal('show');
            $("#reviewSubmitted").on("hidden.bs.modal", function () {
                const form = document.getElementById('reviewForm');
                form.submit();
            });
                return false;            
        }
}

function setSubmitOK() {
    const form = document.getElementById('reviewForm');
    form.submit();
}


//Ad to cart AJAX without refreshin
function AddItemToCart(ProductName) {     
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Gallery/PassToCart");
    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            // receive response from server
            if (this.status == 200) {

                //alert("Item added to your cart");
                $('#ItemAdded').modal('show');
                let cartcontent = document.getElementById("currentCartContents");
                let currentVal = parseInt(cartcontent.innerHTML);            
                cartcontent.innerHTML = currentVal + 1;

                return;
            }
            // convert from JSON string to JavaScript object
            let data = JSON.parse(this.responseText);
            // check availability response
            if (data.isOkay == false) {
                alert("Unable to Add to Cart. Try again");
            }
        }
    }
    let data = {
        "ProductName": ProductName
    };
    xhr.send(JSON.stringify(data))
}

function RemoveFromCart(ProductName, fieldName, priceFieldName, TotalPriceFielddName) {

    let valField = document.getElementById(fieldName);    
    let currenFieldVal = parseInt(valField.innerHTML);

    //minus only if quantity is greater than 0
    if (currenFieldVal > 1) {

        let xhr = new XMLHttpRequest();
        xhr.open("POST", "/Cart/RemoveItem");
        xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");
        xhr.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE) {
                // receive response from server
                if (this.status == 200) {

                    //update cart quantity
                    let cartcontent = document.getElementById("currentCartContents");
                    let currentVal = parseInt(cartcontent.innerHTML);
                    cartcontent.innerHTML = currentVal - 1;
                    //update row quantity
                    valField.innerHTML = currenFieldVal - 1;

                    //update total price for the product                    
                    var qty = document.getElementById(fieldName).innerHTML;                  
                    var price = parseFloat(document.getElementById(priceFieldName).innerHTML.replace("$", "").replace(",", ""));
                    var totalProductPrice = qty * price;
                    document.getElementById(TotalPriceFielddName).innerHTML =
                        '$' + totalProductPrice.toLocaleString(undefined, {
                            minimumFractionDigits: 2, maximumFractionDigits: 2
                        });
                    //update GrandTotal
                    var GrandTotal = parseFloat(document.getElementById("grandTotal").innerHTML.replace("$", "").replace(",", ""));;

                    GrandTotal = GrandTotal - price;
                    document.getElementById("grandTotal").innerHTML =
                        '$' + GrandTotal.toLocaleString(undefined, {
                            minimumFractionDigits: 2, maximumFractionDigits: 2
                        });
                    return;
                 
                    return;
                }
                // convert from JSON string to JavaScript object
                let data = JSON.parse(this.responseText);
                // check availability response
                if (data.isOkay == false) {
                    alert("Unable to substract from Cart. Try again");
                }
            }
        }
        let data = {
            "ProductName": ProductName
        };
        xhr.send(JSON.stringify(data))
    }
    else
        return;
}

function AddtoCart(ProductName, fieldName, priceFieldName, TotalPriceFielddName)
{
    
        let valField = document.getElementById(fieldName);
        let currenFieldVal = parseInt(valField.innerHTML);
   
         let xhr = new XMLHttpRequest();
        xhr.open("POST", "/Cart/AddItem");
        xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");
        xhr.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE) {
                 //receive response from server
                if (this.status == 200) {
                     
                    let cartcontent = document.getElementById("currentCartContents");
                    let currentVal = parseInt(cartcontent.innerHTML);
                    cartcontent.innerHTML = currentVal + 1;
                    valField.innerHTML = currenFieldVal + 1;

                    //update total price for the product                    
                    var qty = document.getElementById(fieldName).innerHTML;
                    var price = parseFloat(document.getElementById(priceFieldName).innerHTML.replace("$", "").replace(",", ""));
                    var totalProductPrice = qty * price;
                    document.getElementById(TotalPriceFielddName).innerHTML =
                        '$' + totalProductPrice.toLocaleString(undefined, {
                        minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    //update GrandTotal
                    var GrandTotal = parseFloat(document.getElementById("grandTotal").innerHTML.replace("$", "").replace(",", ""));;
                    
                    GrandTotal = GrandTotal + price;
                    document.getElementById("grandTotal").innerHTML =
                        '$' + GrandTotal.toLocaleString(undefined, {
                            minimumFractionDigits: 2, maximumFractionDigits: 2});
                    return;
                }
                // convert from JSON string to JavaScript object
                let data = JSON.parse(this.responseText);
                // check availability response
                if (data.isOkay == false) {
                    alert("Unable to Add to Cart. Try again");
                }
            }
        }
        let data = {
            "ProductName": ProductName
    };
    xhr.send(JSON.stringify(data))   
}

function RemoveProduct(ProductName, rowid, fieldName, TotalPriceFieldName)
{
    let xhr = new XMLHttpRequest();
        xhr.open("POST", "/Cart/RemoveProduct");
        xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");
        xhr.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE) {
                // receive response from server
                if (this.status == 200) {

                    //get the quanty to reduce from the cart display from the row that to be deleted
                    let valField = document.getElementById(fieldName);
                    let currenFieldVal = parseInt(valField.innerHTML);

                    //update the cart
                    let cartcontent = document.getElementById("currentCartContents");
                    let currentVal = parseInt(cartcontent.innerHTML);
                    cartcontent.innerHTML = currentVal - currenFieldVal;
                    
                    var TotalDeletedPrice = parseFloat(document.getElementById(TotalPriceFieldName).innerHTML.replace("$", "").replace(",", ""));
                    
                    //update grand total                    
                   
                    var GrandTotal = parseFloat(document.getElementById("grandTotal").innerHTML.replace("$", "").replace(",", ""));;
                    GrandTotal = GrandTotal - TotalDeletedPrice;
                    document.getElementById("grandTotal").innerHTML =
                        '$' + GrandTotal.toLocaleString(undefined, {
                            minimumFractionDigits: 2, maximumFractionDigits: 2
                        });
                    //disable checkout button if Grand total is $0.00
                    if (GrandTotal == 0) {
                        const CheckoutbuttonTop = document.getElementById("CheckoutbuttonTop");
                        CheckoutbuttonTop.disabled = true;
                        const checkoutBtnBottom = document.getElementById("checkoutBtnBottom");
                        checkoutBtnBottom.disabled = true;
                    }

                    //delete the table row of product to be deleted 
                    var row = document.getElementById(rowid);
                    row.parentNode.removeChild(row);
                    return;
                }
                 //convert from JSON string to JavaScript object
                let data = JSON.parse(this.responseText);
                // check availability response
                if (data.isOkay == false) {
                    alert("Unable to Remove product from Cart. Try again");
                }
            }
        }
        let data = {
            "ProductName": ProductName
        };
        xhr.send(JSON.stringify(data))  
}














