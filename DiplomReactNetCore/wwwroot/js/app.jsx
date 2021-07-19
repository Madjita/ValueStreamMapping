
import OrderProduction from './orderProduction.jsx'

import CardVSM from './CardVSM.jsx'

class Phone extends React.Component {

    constructor(props) {
        super(props);
        this.state = { data: props.phone };
        this.onClick = this.onClick.bind(this);
    }
    onClick(e) {
        this.props.onRemove(this.state.data);
    }
    render() {
        return <div>
            <p><b>{this.state.data.name}</b></p>
            <p>Цена {this.state.data.price}</p>
            <p><button onClick={this.onClick}>Удалить</button></p>
        </div>;
    }
}

class PhoneForm extends React.Component {

    constructor(props) {
        super(props);
        this.state = { name: "", price: 0 };

        this.onSubmit = this.onSubmit.bind(this);
        this.onNameChange = this.onNameChange.bind(this);
        this.onPriceChange = this.onPriceChange.bind(this);
    }
    onNameChange(e) {
        this.setState({ name: e.target.value });
    }
    onPriceChange(e) {
        this.setState({ price: e.target.value });
    }
    onSubmit(e) {
        e.preventDefault();
        var phoneName = this.state.name.trim();
        var phonePrice = this.state.price;
        if (!phoneName || phonePrice <= 0) {
            return;
        }
        this.props.onPhoneSubmit({ name: phoneName, price: phonePrice });
        this.setState({ name: "", price: 0 });
    }
    render() {
        return (
            <form onSubmit={this.onSubmit}>
                <p>
                    <input type="text"
                        placeholder="Модель телефона"
                        value={this.state.name}
                        onChange={this.onNameChange} />
                </p>
                <p>
                    <input type="number"
                        placeholder="Цена"
                        value={this.state.price}
                        onChange={this.onPriceChange} />
                </p>
                <input type="submit" value="Сохранить" />
            </form>
        );
    }
}

///////






//////


class PhonesList extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            phones: [],
            simulation: false,
            value: 0,
        };

        this.onAddPhone = this.onAddPhone.bind(this);
        this.onRemovePhone = this.onRemovePhone.bind(this);
    }
    // загрузка данных
    loadData() {
        var xhr = new XMLHttpRequest();
        xhr.open("get", this.props.apiUrl, true);
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ phones: data });
        }.bind(this);
        xhr.send();
    }
    componentWillUnmount() {
        clearInterval(this.interval);
    }



  

    componentDidMount() {
        this.interval = setInterval(() => {
            let newValue = this.state.value + 1;

            if (newValue > this.state.max) {
                newValue = 0;
            }

            this.loadData();
            this.loadSim();

            this.setState({ value: newValue }) //Math.floor(Math.random() * this.state.height)
        }, 1000)
        
    }



        // добавление объекта
    onAddPhone(phone) {
        if (phone) {

            const data = new FormData();
            data.append("name", phone.name);
            data.append("price", phone.price);
            var xhr = new XMLHttpRequest();

            xhr.open("post", this.props.apiUrl, true);
            xhr.onload = function () {
                if (xhr.status === 200) {
                    this.loadData();
                }
            }.bind(this);
            xhr.send(data);
        }
    }
        // удаление объекта
    onRemovePhone(phone) {

        if (phone) {
            var url = this.props.apiUrl + "/" + phone.id;

            var xhr = new XMLHttpRequest();
            xhr.open("delete", url, true);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onload = function () {
                if (xhr.status === 200) {
                    this.loadData();
                }
            }.bind(this);
            xhr.send();
        }
    }

    handleOnClick = (e) => {
        var url = "/api/simulation";
        const data = new FormData();
        data.append("start", !this.state.simulation);

        var xhr = new XMLHttpRequest();
        xhr.open("post", url, true);
        xhr.onload = function () {
            if (xhr.status === 200) {
                console.log("xhr.responseText = ", JSON.parse(xhr.responseText))
                this.loadSim();
            }
        }.bind(this);
        xhr.send(data);
    }


    loadSim = () => {
        var url = "/api/simulation";

        var xhr = new XMLHttpRequest();
        xhr.open("get", url, true);
        xhr.onload = function () {
            let sim = JSON.parse(xhr.responseText);
            //console.log("SIM =", sim)
            this.setState({ simulation: sim });
        }.bind(this);
        xhr.send();
    }


   

    render() {

        var remove = this.onRemovePhone;
        return (
            <div>
            <OrderProduction apiUrl='/api/Production' apiUrl2='/api/order' />
            <PhoneForm onPhoneSubmit={this.onAddPhone} />
            <CardVSM apiUrl='/api/manufacture'/>
            <a onClick={this.handleOnClick}>Проверка</a>
            <h1>Симуляция {this.state.simulation ? 'Включенна' : 'Выключенна'}</h1>
            <h2>Список смартфонов</h2>
            <div>
                {
                    this.state.phones.map(function (phone) {
                        return <Phone key={phone.id} phone={phone} onRemove={remove} />
                    })
                }
            </div>
            </div>
        )
    }
}

ReactDOM.render(
    <PhonesList apiUrl="/api/phones" />,
    document.getElementById("content")
);