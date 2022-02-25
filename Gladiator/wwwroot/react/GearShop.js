

function GearShop() {
    const [hostPort, setHostPort] = React.useState('44362');
    const [id, setId] = React.useState(parseInt(gladiatorId));
    const [error, setError] = React.useState(null);
    const [isLoaded, setIsLoaded] = React.useState(false);
    const [allModifiers, setAllModifiers] = React.useState([]);
    const [gladiatorModifiers, setGladiatorModifiers] = React.useState([]);
    const [buyModifiers, setBuyModifiers] = React.useState('Select Gear');
    const [gold, setGold] = React.useState(null);
    const [selectedModifier, setSelectedModifier] = React.useState([]);
    const [buyResult, setBuyResult] = React.useState('');
    const[reRender, setReRender] = React.useState(1);




        
    React.useEffect(() => {
        GetGladiatorVM();
    }, [reRender])

    
    function GetGladiatorVM() {
        fetch("https://localhost:" + hostPort + "/playergladiators/getmodifiers/" + id)
            .then(res => res.json())
            .then( result => {
                setIsLoaded(true);
                setGladiatorModifiers(result.gladiatorModifiers);
                    setAllModifiers(result.allModifiers);
                    console.log(allModifiers);
                    setGold(result.gold);
                    setSelectedModifier(result.allModifiers[0]);
                }).
                then ((error) => {
                    setIsLoaded(false);
                    setError(error);
                    console.log(error);
        });
        
        
    }

    let handleModifierChange = (e) => {
        setBuyModifiers(e.target.value);
        setSelectedModifier(allModifiers.find( mod => mod.id === parseInt(event.target.value)));
    }

    function BuyGear () {
        if (selectedModifier.price > gold) {
            setBuyResult('Not enough gold.')
        }
        else {
            if (gladiatorModifiers.some(mod => mod.name === selectedModifier.name))
            {
                    setBuyResult('Gear already owned.');
            }
            else {

                fetch("https://localhost:" + hostPort + "/playergladiators/buygear", {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ id: id, modifierId: selectedModifier.id})
                })
                    .then(response => {
                        console.log('Response:', response);
                    })
                    .then(data => {
                        console.log('Success:', data);
                    })
                    .catch((error) => {
                        console.error('Error:', error);
                    });

                setBuyResult('Item bought.')
                setTimeout(() => { setReRender(reRender + 1); }, 1000);
         
            }
            
        }
    }
    

    return (
        <div>
            <div>
                <h3>Gladiators gear</h3>
                <div>
                    <table className="table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Str</th>
                                <th>HP</th>
                                <th>XP</th>
                                <th>Def</th>
                                <th>Description</th>
                                
                            </tr>
                        </thead>
                        <tbody>
                            {gladiatorModifiers.length > 0 ? (
                                gladiatorModifiers.map((item) => (
                                    <tr key={item.id}>
                                        <td >{item.name}</td>
                                        <td >{item.str}</td>
                                        <td >{item.hp}</td>
                                        <td >{item.xp}</td>
                                        <td >{item.def}</td>
                                        <td >{item.description}</td>
                                    </tr>
                                ))
                            ) : (
                                <tr>
                                    <td colSpan={3}>No modifiers</td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
                
                <div className="mt-4">
                    <h2>Buy gear for gladiator</h2>
                    <h3>Current gold: {gold} </h3>
                    <div className="form-group mt-3">
                        <select className="btn btn-primary dropdown-toggle" onChange={handleModifierChange}>
                            <option value="Select Gear"> -- Select Gear To buy -- </option>
                            {allModifiers.map((modifier) => <option key={modifier.id} value={modifier.id}>{modifier.name} {modifier.price} gold  </option>)}
                        </select>
                    </div>

                    <h3>Description</h3>
                    <p>{selectedModifier.description}</p>
                    <div>
                        <button onClick={BuyGear} className="btn btn-primary">Buy gear</button>
                    </div>
                    <div>
                        <p>{buyResult}</p>
                    </div>
                </div>
            </div>

        </div>

    );

}

ReactDOM.render(<GearShop />, document.getElementById('root'))