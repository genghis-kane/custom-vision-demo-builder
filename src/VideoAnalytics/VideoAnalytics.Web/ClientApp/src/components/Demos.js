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
      selectedDemoIndex: 0
    };

    //TODO: appsettings would be nice...
    this.endangeredSpeciesComponentKey = 'endangeredspecies';
    this.endangeredSpeciesVideoUrl = 'https://videoanalyticsdemosstora.blob.core.windows.net/demos/endangeredspecies/video.mp4?sp=rl&st=2020-10-23T08:40:58Z&se=2026-10-24T08:40:00Z&sv=2019-12-12&sr=c&sig=giK63CoU42DzktxC9q34N6gSd1%2F3%2BORbVFmhoQqi%2Fis%3D';
    this.endangeredSpeciesPredictionsUrl = 'https://videoanalyticsdemosstora.blob.core.windows.net/demos/endangeredspecies/predictions.json?sp=rl&st=2020-10-23T08:40:17Z&se=2026-10-24T08:40:00Z&sv=2019-12-12&sr=c&sig=6zi5x1ilF%2BVaKaRN8d7ZOJTcqkCO7sVV3%2BEGDgwCT3Y%3D';
    this.endangeredSpeciesBoundingBoxColor = 'red';
  }

  render() {
    return (
      <div className="demos-container">
        <h1>Demos!</h1>
        <p>Some information about these demos.....</p>

        <Tabs defaultTab="demo-tab-one" vertical>
          <TabList>
            <Tab tabFor="demo-tab-one">Endangered species</Tab>
            <Tab tabFor="demo-tab-two">Health and safety</Tab>
            <Tab tabFor="demo-tab-three">Supply chain</Tab>
          </TabList>
        <TabPanel tabId="demo-tab-one">
          <h2>Endangered species</h2>
          <p>Some information about this demo.</p>

          <Demo key={this.endangeredSpeciesComponentKey} videoUrl={this.endangeredSpeciesVideoUrl} predictionResultsUrl={this.endangeredSpeciesPredictionsUrl} boundingBoxColor={this.endangeredSpeciesBoundingBoxColor} />
        </TabPanel>
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
}
