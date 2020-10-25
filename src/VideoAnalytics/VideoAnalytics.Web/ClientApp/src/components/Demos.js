import React, { Component } from 'react';
import { Tabs, Tab, TabPanel, TabList } from 'react-web-tabs';
import { Demo } from './Demo';

import 'react-web-tabs/dist/react-web-tabs.css';
import './Demos.css';

export class Demos extends Component {
  static displayName = Demos.name;

  constructor(props) {
    super(props);
    this.state = { 
      settings: [],
      loading: false
    };

    this.populateSettings = this.populateSettings.bind(this);

    this.endangeredSpeciesComponentKey = 'endangeredspecies';
    this.endangeredSpeciesBoundingBoxColor = 'red';
  }

  componentDidMount() {
    this.populateSettings();
  }

  render() {
    let endangeredSpeciesSettings = this.state.settings.find(s => s.name === this.endangeredSpeciesComponentKey);
    // let healthAndSafetySettings = this.state.settings.find(s => s.name === this.healthAndSafetyComponentKey);
    // let supplyChainSettings = this.state.settings.find(s => s.name === this.supplyChainComponentKey);

    return (
      <div className="demos-container">
        <h1>Demos!</h1>
        <p>Some information about these demos.....</p>

        <Tabs defaultTab="demo-tab-one" vertical>
          <TabList>
            {endangeredSpeciesSettings && (
              <Tab tabFor="demo-tab-one">Endangered species</Tab>
            )}
            <Tab tabFor="demo-tab-two">Health and safety</Tab>
            <Tab tabFor="demo-tab-three">Supply chain</Tab>
          </TabList>

          {endangeredSpeciesSettings && (  
            <TabPanel tabId="demo-tab-one">
              <h2>Endangered species</h2>
              <p>Some information about this demo.</p>

              <Demo key={this.endangeredSpeciesComponentKey} 
                    videoUrl={endangeredSpeciesSettings.videoUrl} 
                    predictionResultsUrl={endangeredSpeciesSettings.predictionsUrl} 
                    boundingBoxColor={this.endangeredSpeciesBoundingBoxColor} />
            </TabPanel>
          )}
        <TabPanel tabId="demo-tab-two">
          <h2>Health and safety</h2>
          <p>Some information about this demo.</p>
        </TabPanel>
        <TabPanel tabId="demo-tab-three">
          <h2>Supply chain</h2>
          <p>Some information about this demo.</p>
        </TabPanel>
      </Tabs>
      </div>
    );
  }

  async populateSettings() {
    this.setState({ settings: [], loading: true });

    const response = await fetch('demos');
    const data = await response.json();
    
    this.setState({ settings: data.settings, loading: false });
  }
}
