import React, { Component } from 'react';
import ReactTooltip from 'react-tooltip';

export class VideoImageFrameExtractor extends Component {
  static displayName = VideoImageFrameExtractor.name;

  constructor(props) {
    super(props);
    this.state = { 
      videoFile: null,
      frameStepMilliseconds: 300,
      maxDurationMilliseconds: 10000,
      loading: false,
    };

    this.handleInputChange = this.handleInputChange.bind(this);
  }

  render() {
    return (
      <div className={this.props.containerClass}>
        <div style={{ display: this.state.loading ? 'block' : 'none' }}>
          <p>Loading...</p>
        </div>

        <div style={{ display: !this.state.loading ? 'block' : 'none' }}>
          { this.props.formSectionTitle && <p className="step-title">{this.props.formSectionTitle}</p> }
          { this.props.formSectionBlurb && <p>{this.props.formSectionBlurb}</p> }

          <form onSubmit={e => this.submit(e)}>
            <div className="form-group">
              <label for='videoFile' data-tip='Upload a video to extract image frames from'>
                Upload video file: 
              </label>
              <ReactTooltip />
              <input 
                id='videoFile' 
                name='videoFile' 
                type='file' 
                className='form-control-file' 
                onChange={this.handleInputChange} />
            </div>
            <div className="form-group">
              <label for='frameStepMilliseconds' data-tip='Extract an image frame every [x] milliseconds'>
                Frame step (ms): 
              </label>
              <input 
                id='frameStepMilliseconds' 
                name='frameStepMilliseconds' 
                type='text' 
                className='form-control' 
                value={this.state.frameStepMilliseconds} 
                onChange={this.handleInputChange} />
              <ReactTooltip />
            </div>
            <div className="form-group">
              <label for='maxDurationMilliseconds' data-tip='Stop processing after this number of milliseconds'>
                Max duration (ms): 
              </label>
              <input 
                id='maxDurationMilliseconds' 
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
    );
  }

  handleInputChange(event) {
    const target = event.target;
    const value = target.type === 'file' ? target.files[0] : target.value;
    const name = target.name;

    this.setState({
      [name]: value
    });
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

    this.props.callback(imageFrames);
  }
}
