import React, { Component } from 'react';

import { CustomVisionProjectSelector } from './form/CustomVisionProjectSelector';
import { VideoImageFrameExtractor } from './form/VideoImageFrameExtractor';
import { VideoImageFramePublisher } from './form/VideoImageFramePublisher';

import './VideoFrameExtractor.css';

export class VideoFrameExtractor extends Component {
  static displayName = VideoFrameExtractor.name;

  constructor(props) {
    super(props);
    this.state = { 
      step: 0,
      frames: []
    };

    this.customVisionProjectSelectCallback = this.customVisionProjectSelectCallback.bind(this);
    this.extractVidoFramesCallback = this.extractVidoFramesCallback.bind(this);
  }

  render() {
    return (
      <div className='row'>
        <h1 id='tabelLabel'>Extract image frames</h1>
        <div className='container'>
          <div class='row'>
            <p>This tool can be used to extract image frames out of an uploaded video. You will be given the option to select which extracted frames should be published up to your Custom Vision project. From there, they can be used to train your AI model.</p>
          </div>
         
          <div style={{ display: (this.state.step === 0) ? 'block' : 'none' }}>
            <div class='row'>
              <CustomVisionProjectSelector
                callback={this.customVisionProjectSelectCallback}
                containerClass='step-container'
                formSectionTitle='Step 1: Select or create a custom vision project'
                formSectionBlurb='Choose an existing custom vision project, or type a name into the box to create a new custom vision project for your extracted image frames to be loaded into.' />
            </div>
          </div>
          
          <div style={{ display: (this.state.step === 1) ? 'block' : 'none' }}>
            <div class='row'>
              <VideoImageFrameExtractor
                callback={this.extractVidoFramesCallback}
                containerClass='step-container'
                formSectionTitle='Step 2: Upload your video file'
                formSectionBlurb='Upload a video file to extract image frames from. These will be published to the custom vision project you selected in the last step, to be used for training.' />
            </div>
          </div>
         
          <div style={{ display: (this.state.step === 2) ? 'block' : 'none' }}>
            <div className='row'>
              <VideoImageFramePublisher 
                images={this.state.frames}
                containerClass='step-container'
                formSectionTitle='Step 3: Publish images'
                formSectionBlurb='Choose which image frames to publish to your custom vision project.' />
            </div>
          </div>

        </div>
      </div>
    );
  }

  customVisionProjectSelectCallback(result) {
    this.setState({ step: 1 });
  }

  extractVidoFramesCallback(result) {
    this.setState({ frames: result });
    this.setState({ step: 2 });
  }
}
