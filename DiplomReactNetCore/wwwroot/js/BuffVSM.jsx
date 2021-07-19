class BuffVSM extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            buff: null,
        };
    }
    // загрузка данных
    loadData() {
        var xhr = new XMLHttpRequest();
        xhr.open("get", this.props.apiUrl, true);
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ buff: data });
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

        var remove = this.onRemovePhone;
        let { buff } = this.state

        let value;
        let name;
        if (buff != null) {
            value = <p>{buff.value}</p>
            name = <h3>{buff.name}</h3>
        }
        else {
            return null;
        }

        return <div className="item">
            {name}
            <div>
                {value}
            </div>
        </div>
    }
}


export default BuffVSM;