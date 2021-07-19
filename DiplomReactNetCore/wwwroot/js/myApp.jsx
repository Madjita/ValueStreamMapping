import CardVSM from './CardVSM.jsx'
import Orders from './Orders.jsx'
import OrdersFineshedWork from './OrdersFineshedWork.jsx'

class Content extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            update: false
        };
    }

    onOrderSubmit = () => {
        console.log("onOrderSubmit ");
        this.setState({ update: !this.state.update });
    }

    render() {
        return (
            <div className='myContent'>
                <Orders apiUrl='/api/order' update={this.state.update} />
                <CardVSM apiUrl='/api/manufacture' onOrderSubmit={this.onOrderSubmit} />
                <OrdersFineshedWork apiUrl='/api/orderfinishedwork' />
            </div>
        )
    }
}

ReactDOM.render(
    <Content/>,
    document.getElementById("content")
);