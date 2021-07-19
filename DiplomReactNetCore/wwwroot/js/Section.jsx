import BuffVSM from './BuffVSM.jsx'
import EtapVSM from './EtapVSM.jsx'


function NumberList(props) {
    const numbers = props.numbers;
    const listItems = numbers.map((item, index) => 
        <li key={index} style={item.wait ? { backgroundColor: 'yellow'} : {}} >
            <div>{item.production.name} ({item.quantity})</div>
            <div>{item.timeActual}</div>
        </li>
    );
    return (
        <ul>{listItems}</ul>);
}


class QueuqOrders extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            orders: [],
        };
    }

    // загрузка данных
    loadData() {
        var xhr = new XMLHttpRequest();
        xhr.open("get", this.props.apiUrl, true);
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ orders: data });
        }.bind(this);
        xhr.send();
    }

    componentWillUnmount() {
        clearInterval(this.interval);
    }

    componentDidMount() {
        this.interval = setInterval(() => {
            this.loadData();
        }, 1000)

    }

    render() {
        let { orders } = this.state
        return (
            <div>
                <NumberList numbers={orders} />
            </div>
        );
    }
};



class Section extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            section: this.props.section,
        };
    }

    render() {

        let sections = this.state.section.map((item, index) => {
            let buffer = null;
            if (item.buf != null) {
                buffer = <BuffVSM  apiUrl={'/api/buffervsm/' + item.buf.name} className="item"/>
            }
            let etap = <EtapVSM apiUrl={'/api/etapvsm/' + item.etap.id}  />

            return (
                <div key={index} className="box shadow">
                    <QueuqOrders apiUrl={'/api/manufacture/' + item.etap.id} />
                    {buffer}
                    {etap}
                </div>
                )
            })

                    return (
             <div className="item shadow">
                    {sections}
             </div>
        ) 
    }

}

export default Section;