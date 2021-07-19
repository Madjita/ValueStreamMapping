class OrderItem extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            item: this.props.item
        };
    }


    componentDidUpdate(prevProps) {
        if (this.props.item.timeActual !== prevProps.item.timeActual) {
            this.setState({ item: this.props.item });
        }
    }



    onClick = (e) => {
        e.preventDefault();

        let { item } = this.state;

        item.simulation = !item.simulation

        let object = {
            "Id": item.id,
            "ProductionName": item.name,
            "Start": item.simulation
        }


        var xhr = new XMLHttpRequest();
        xhr.open("post", this.props.apiUrl, true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.onload = function () {
        }.bind(this);


        xhr.send(JSON.stringify(object));


        //this.sendPost({ quantity: quantityCount, name: this.props.name });
        this.setState({ item: item });
    }

    render() {
        let { item } = this.state;

        console.log("item = ", item);
        let text = item.simulation ? "Выключить Симуляцию" : "Включить Симуляцию";
        return( <div>
            <p>Имя: {item.name}</p>
            <p>Количество: {item.quantity}</p>
            <p>Актуальное время: {item.timeActual}</p>
            <input onClick={this.onClick} type="submit" value={text} />
        </div>
        )

    }

};

class Orders extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            orders: [],
        };
    }

    componentWillUnmount() {
        clearInterval(this.interval);
    }

    componentDidMount() {
        this.interval = setInterval(() => {
            this.loadData();
        }, 1000)
    }

    componentDidUpdate(prevProps) {
        console.log(this.props.update,prevProps.update)
        if (this.props.update !== prevProps.update) {
            this.loadData();
        }
    }


    loadData() {
        var xhr = new XMLHttpRequest();
        xhr.open("get", this.props.apiUrl, true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            console.log("OrderItem =",data);
            this.setState({ orders: data });
        }.bind(this);
        xhr.send();
    }

    render() {
        let { orders } = this.state;
        return (
            <div>
                {
                    orders.map((item, index) => {
                        return <OrderItem key={index} item={item} apiUrl='/api/simulation' />
                     })
                }
            </div>
        )
    }
};

export default Orders;