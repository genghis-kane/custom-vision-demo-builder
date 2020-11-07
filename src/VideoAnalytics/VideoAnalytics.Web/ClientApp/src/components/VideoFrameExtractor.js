import React, { Component } from 'react';
import CreatableSelect from 'react-select/creatable';

import { VideoImageFrameExtractor } from './form/VideoImageFrameExtractor';
import { VideoImageFramePublisher } from './form/VideoImageFramePublisher';

import './VideoFrameExtractor.css';

export class VideoFrameExtractor extends Component {
  static displayName = VideoFrameExtractor.name;

  constructor(props) {
    super(props);
    this.state = { 
      step: 0,
      existingProjects: [],
      selectedProject: '',
      frames: [],
      loading: false,
    };

    this.extractVidoFramesCallback = this.extractVidoFramesCallback.bind(this);
    this.getCustomVisionProjects = this.getCustomVisionProjects.bind(this);
  }

  componentDidMount() {
    this.getCustomVisionProjects();
  }

  render() {
    return (
      <div className='row'>
        <h1 id='tabelLabel'>Extract image frames</h1>
        <div className='container'>
          <div class='row'>
            <p>This tool can be used to extract image frames out of an uploaded video. You will be given the option to select which extracted frames should be published up to your Custom Vision project. From there, they can be used to train your AI model.</p>
          </div>
          <div style={{ display: this.state.loading ? 'block' : 'none' }}>
            <div className='row'>
              <p>Loading...</p>
            </div>
          </div>

          <div style={{ display: (!this.state.loading && this.state.step === 0) ? 'block' : 'none' }}>
            <div className='row'>
              <div className='step-container'>
                <form onSubmit={e => this.submitProject(e)}>
                  <p className="step-title">Step 1: Select or create a custom vision project</p>
                  <p>Choose an existing custom vision project, or type a name into the box to create a new custom vision project for your extracted image frames to be loaded into.</p>
              
                  <div className="form-group">
                    <label for='projectName'>Project name: </label>
                    <CreatableSelect
                      id='projectName'
                      name='projectName'
                      isClearable
                      value={this.state.selectedProject}
                      onChange={this.handleProjectSelect}
                      onInputChange={this.handleProjectCreate}
                      options={this.state.existingProjects}
                    />
                  </div>
                  <button type='submit' className='btn btn-primary'>Submit</button>
                </form>
              </div>
            </div>
          </div>
          
          <div style={{ display: (!this.state.loading && this.state.step === 1) ? 'block' : 'none' }}>
            <div class='row'>
              <VideoImageFrameExtractor
                callback={this.extractVidoFramesCallback}
                containerClass='step-container'
                formSectionTitle='Step 2: Upload your video file'
                formSectionBlurb='Upload a video file to extract image frames from. These will be published to the custom vision project you selected in the last step, to be used for training.' />
            </div>
          </div>
         
          <div style={{ display: (!this.state.loading && this.state.step === 2) ? 'block' : 'none' }}>
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

  async getCustomVisionProjects() {
    const response = await fetch('customvisionauthoring/listcvprojects');
    const data = await response.json();

    const cvProjects = data.map((p) => {
      return {
        value: p,
        label: p
      }
    });

    this.setState({ existingProjects: cvProjects });
  }

  handleProjectSelect = selectedProject => {
    this.setState({ selectedProject });
  };

  async submitProject(e) {
    e.preventDefault();
    this.setState({ loading: true });

    const requestBody = { projectName: this.state.selectedProject.value };

    const response = await fetch('customvisionauthoring/createcvproject', {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(requestBody)
    });

    const data = await response.json();

    if (data.success) {
      this.setState({ step: 1 });
    }
    this.setState({ loading: false });
  }

  extractVidoFramesCallback(result) {
    this.setState({ frames: result });
    this.setState({ step: 2 });
  }
}
