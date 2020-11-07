import React, { Component } from 'react';
import CreatableSelect from 'react-select/creatable';
import ReactTooltip from 'react-tooltip';

import { ImagePublisher } from './form/ImagePublisher';

import './VideoFrameExtractor.css';

export class VideoFrameExtractor extends Component {
  static displayName = VideoFrameExtractor.name;

  constructor(props) {
    super(props);
    this.state = { 
      step: 0,
      existingProjects: [],
      selectedProject: '',
      videoFile: null,
      frameStepMilliseconds: 300,
      maxDurationMilliseconds: 10000,
      frames: [],
      loading: false,
    };

    this.getCustomVisionProjects = this.getCustomVisionProjects.bind(this);
    this.handleInputChange = this.handleInputChange.bind(this);
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
              <div className='step-container'>
                <p className="step-title">Step 2: Upload your video file</p>
                <p>Upload a video file to extract image frames from. These will be published to the custom vision project you selected in the last step, to be used for training.</p>
              
                <form onSubmit={e => this.submit(e)}>
                  <div className="form-group">
                    <label for='videoFile' 
                          data-tip='Upload a video to extract image frames from'>
                            Upload video file: 
                    </label>
                    <ReactTooltip />
                    <input id='videoFile' 
                          name='videoFile'
                          type='file' 
                          className='form-control-file'
                          onChange={this.handleInputChange} />
                  </div>
                  <div className="form-group">
                    <label for='frameStepMilliseconds' 
                          data-tip='Extract an image frame every [x] milliseconds'>
                            Frame step (ms): 
                    </label>
                    <input id='frameStepMilliseconds' 
                          name='frameStepMilliseconds' 
                          type='text' 
                          className='form-control' 
                          value={this.state.frameStepMilliseconds} 
                          onChange={this.handleInputChange} />
                    <ReactTooltip />
                  </div>
                  <div className="form-group">
                    <label for='maxDurationMilliseconds' 
                          data-tip='Stop processing after this number of milliseconds'>
                            Max duration (ms): 
                    </label>
                    <input id='maxDurationMilliseconds' 
                          name='maxDurationMilliseconds' 
                          type='text' 
                          className='form-control' 
                          value={this.state.maxDurationMilliseconds} 
                          onChange={this.handleInputChange} />
                    <ReactTooltip />
                  </div>
                  <button type='submit' className='btn btn-primary'>Upload</button>
                </form>
              </div>
            </div>
          </div>
         
          <div style={{ display: (!this.state.loading && this.state.step === 2) ? 'block' : 'none' }}>
            <div className='row'>
              <ImagePublisher 
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

  handleInputChange(event) {
    const target = event.target;
    const value = target.type === 'file' ? target.files[0] : target.value;
    const name = target.name;

    this.setState({
      [name]: value
    });
  }

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

  async submit(e) {
    e.preventDefault();
    this.setState({ loading: true });

    const formData = new FormData();
    formData.append('file', this.state.videoFile);
    formData.append('frameStepMilliseconds', this.state.frameStepMilliseconds);
    formData.append('maxDurationMilliseconds', this.state.maxDurationMilliseconds);
    
    const response = await fetch('customvisionauthoring/uploadvideo', {
      method: 'POST',
      headers: {
        'Accept': 'application/json'
      },
      body: formData
    });

    const data = await response.json();

    const imageFrames = data.map((imageFrame) => {
      return {
        src: imageFrame,
        thumbnail: imageFrame,
        thumbnailHeight: 200,
        thumbnailWidth: 330,
        isSelected: true
      }
    });

    this.setState({ frames: imageFrames, step: 2, loading: false });
  }
}
