import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { VideoFrameExtractor } from './components/VideoFrameExtractor';
import { Predict } from './components/Predict';
import { Demos } from './components/Demos';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/train' component={VideoFrameExtractor} />
        <Route path='/predict' component={Predict} />
        <Route path='/demos' component={Demos} />
      </Layout>
    );
  }
}
