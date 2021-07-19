

import Section from './Section.jsx'
import OrderProduction from './orderProduction.jsx'

class CardVSM extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            cardVSMs: [],
        };
    }

    loadData() {
        var xhr = new XMLHttpRequest();
        xhr.open("get", this.props.apiUrl, true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ cardVSMs: data });
        }.bind(this);
        xhr.send();
    }
    componentWillUnmount() {
    }

    componentDidMount() {
        this.loadData();
    }



    render() {
        return <div>
                {
                this.state.cardVSMs.map((item, index) => {

                    let sections = item.sections.map((section, index2) => {
                        return <Section key={index2} section={section} />
                    });
                    return ([
                        <div key={index}>
                            <h2>{item.name}</h2>
                            <OrderProduction onOrderSubmit={this.props.onOrderSubmit} name={item.name}/>
                        </div>,
                        <div key={index + 1} className="box">
                            {sections}
                        </div>
                        ])

                    })
                }
        </div>
    }
}


export default CardVSM;