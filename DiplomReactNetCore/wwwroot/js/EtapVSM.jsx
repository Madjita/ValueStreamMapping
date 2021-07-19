class EtapVSM extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            etap: null,
        };
    }
    // загрузка данных
    loadData() {
        var xhr = new XMLHttpRequest();
        xhr.open("get", this.props.apiUrl, true);
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ etap: data });
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
        let { etap } = this.state
        let value;
        let name;
        let inWork;
        let description;
        if (etap != null) {
            name = <h3>{etap.name}</h3>
            description = <p>{etap.description}</p>
            if ('productionName' in etap) {

               // let time = new Date(etap.timeActual).toLocaleString().split(',')[1] //.toUTCString(); //toString('#DD#/#MM#/#YYYY# #hh#:#mm#:#ss#');
                //time = etap.timeActual;
                inWork = (
                    <div>
                        <p>В работе : {etap.productionName} ({etap.quantity})</p>
                        <p>Время работы этапа:      {etap.timeActual}</p>
                        <p>Актуальное время заказа: {etap.timeOrder}</p>
                    </div>

                    )
            }
            else {
                inWork = null;
            }

            
        }
        else {
            return null;
        }

        return <div className="item">
            {name}
            {description}
            {inWork}
        </div>
    }
}


export default EtapVSM;