import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Welcome!</h1>
        <p>This application is designed to help you build Custom Vision demos out of video files.</p>
        <h4>What can I do?</h4>
        <ul>
          <li><strong>Train: </strong>Upload a video to extract a set of image frames that can be used to train a new Custom Vision model.</li>
          <li><strong>Predict: </strong>Upload a brand new video to test against your existing models.</li>
          <li><strong>Demos: </strong>Check out some pre-canned demos in the Endangered Species, Health and Safety, and Supply Chain domains.</li>
        </ul>
      </div>
    );
  }
}
