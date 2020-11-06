import React, { Component } from 'react';
import Select from 'react-select';
import ReactTooltip from 'react-tooltip';
import Gallery from 'react-grid-gallery';

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
    this.renderImageFrames = this.renderImageFrames.bind(this);
    this.onSelectImage = this.onSelectImage.bind(this);
    this.uploadSelectedImages = this.uploadSelectedImages.bind(this);
  }

  componentDidMount() {
    this.getCustomVisionProjects();
  }

  renderImageFrames(frames) {
    return (
      <div className='frame-list-container'>
        <p>Choose frames to upload to the Custom Vision portal:</p>
        <Gallery images={frames} onSelectImage={this.onSelectImage} enableLightbox={false}/>
      </div>
    );
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
              <p className="step-title">Step 1: Select or create a custom vision project</p>
              <p>Choose an existing custom vision project, or type a name into the box to create a new custom vision project for your extracted image frames to be loaded into.</p>
              <form onSubmit={e => this.submitProject(e)}>
                <div className="form-group">
                  <label for='projectName'>Project name: </label>
                  <Select
                    id='projectName'
                    name='projectName'
                    value={this.state.selectedProject}
                    onChange={this.handleProjectUpdate}
                    options={this.state.existingProjects}
                  />
                </div>
                <button type='submit' className='btn btn-primary'>Submit</button>
              </form>
            </div>
          </div>
          
          <div style={{ display: (!this.state.loading && this.state.step === 1) ? 'block' : 'none' }}>
            <div class='row'>
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
         
          <div style={{ display: (!this.state.loading && this.state.step === 2) ? 'block' : 'none' }}>
            <div className='row'>
              <div className='frame-list-container'>
                <p>Choose frames to upload to the Custom Vision portal:</p>
              <Gallery images={this.state.frames} onSelectImage={this.onSelectImage} enableLightbox={false}/>
            </div>
            </div>
            <div className='row'>
              <button type='button' className='btn btn-primary' onClick={this.uploadSelectedImages}>Upload</button>
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

  handleProjectUpdate = selectedProject => {
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

    this.setState({ frames: imageFrames, step: 1, loading: false });
  }

  async uploadSelectedImages() {
    var frames = this.state.frames;
    var requestBody = frames.filter(i => i.isSelected).map(i => i.src);
    
    await fetch('customvisionauthoring/uploadvideoframes', {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(requestBody)
    });
  }

  /* Gallery methods */
  onSelectImage (index, image) {
    var images = this.state.frames.slice();
    var img = images[index];
    if(img.hasOwnProperty("isSelected"))
        img.isSelected = !img.isSelected;
    else
        img.isSelected = true;

    this.setState({
        frames: images
    });

    if(this.allImagesSelected(images)){
        this.setState({
            selectAllChecked: true
        });
    }
    else {
        this.setState({
            selectAllChecked: false
        });
    }
  }

  allImagesSelected (images){
    var f = images.filter(
        function (img) {
            return img.isSelected == true;
        }
    );
    return f.length == images.length;
  }
}
