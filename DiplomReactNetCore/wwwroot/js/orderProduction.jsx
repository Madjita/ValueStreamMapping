class OrderProduction extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            quantity: 0,
            apiUrl2: '/api/order'
        }
        this.onSubmit = this.onSubmit.bind(this);
        this.onQuantityChange = this.onQuantityChange.bind(this);
    };
    onQuantityChange(e) {
        this.setState({ quantity: e.target.value });
    }
    onSubmit(e) {
        e.preventDefault();
        var quantityCount = Number(this.state.quantity);
        if (quantityCount <= 0) {
            return;
        }
        
        this.sendPost({ quantity: quantityCount, name: this.props.name });
        this.setState({ quantity: 0 });
    }

    sendPost = (order) => {
        if (order) {
            let object = {
                "ProductionName": order.name,
                "Quantity": order.quantity
            }

            console.log(object);

            let xhr = new XMLHttpRequest();
            xhr.open("post", this.state.apiUrl2, true);
            xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            xhr.onload = function () {
                if (xhr.status === 200) {
                    this.props.onOrderSubmit({});
                }
            }.bind(this);


            xhr.send(JSON.stringify(object)); 
        }

    }

    render() {
        return (
            <form onSubmit={this.onSubmit}>
                <p>
                    <input type="number"
                        placeholder="Количество партии"
                        value={this.state.quantity}
                        onChange={this.onQuantityChange} />
                </p>
                <input type="submit" value="Добавить" />
            </form>
        );
    }
}

export default OrderProduction;