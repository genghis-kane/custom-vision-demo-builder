import React, { Component } from 'react';
import { Tabs, Tab, TabPanel, TabList } from 'react-web-tabs';
import { Prediction } from './Prediction';

import 'react-web-tabs/dist/react-web-tabs.css';
import './Predictions.css';

export class Predictions extends Component {
  static displayName = Predictions.name;

  constructor(props) {
    super(props);
    this.state = { 
      projects: [],
      loading: false
    };

    this.getCustomVisionProjects = this.getCustomVisionProjects.bind(this);
  }

  componentDidMount() {
    this.getCustomVisionProjects();
  }

  render() {
    return (
      <div className="predictions-container">
        <h1>Make new predictions</h1>
        <p>Upload a video against an existing Custom Vision model to see new predictions.</p>

        <Tabs vertical>
          <TabList>
            {this.state.projects.map(p => {  
              return <Tab tabFor={p}>{p}</Tab>
            })}
          </TabList>

          {this.state.projects.map(p => {  
            return <TabPanel tabId={p}><Prediction customVisionProjectName={p} /></TabPanel>
          })}
        </Tabs>
      </div>
    );
  }

  async getCustomVisionProjects() {
    const response = await fetch('customvisionauthoring/listcvprojects');
    const data = await response.json();
    this.setState({ projects: data });
  }
}
