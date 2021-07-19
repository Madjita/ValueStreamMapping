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

    render() {
        let { item } = this.state;
        return (<div>
            <p>Имя: {item.name}</p>
            <p>Количество: {item.quantity}</p>
            <p>Потраченное время: {item.timeActual}</p>
        </div>
        )

    }

};

class OrdersFineshedWork extends React.Component {
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
        console.log(this.props.update, prevProps.update)
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

export default OrdersFineshedWork;