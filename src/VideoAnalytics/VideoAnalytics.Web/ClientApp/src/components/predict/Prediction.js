import React, { Component } from 'react';
import ReactPlayer from 'react-player'

import './Prediction.css';

export class Prediction extends Component {
  static displayName = Prediction.name;

  /* TODO: polling */
  constructor(props) {
    super(props);

    this.setIntervalId = 0;
    this.boundingBoxColor = 'red';
    this.boundingBoxBorderStyle = `3px solid ${this.boundingBoxColor}`;

    this.state = { 
      file: null, 
      uploadedVideoPath: null, 
      predictions: [],
      currentFramePrediction: {},
      currentFramePredictionIndex: 0,
      loading: false, 
      playing: false,
      videoPlayerHeight: 0,
      videoPlayerWidth: 0
    };

    this.videoPlayerRef = React.createRef();
    this.boundingBoxRef = React.createRef();

    this.renderVideo = this.renderVideo.bind(this);
    this.startVideo = this.startVideo.bind(this);
    this.stopVideo = this.stopVideo.bind(this);
  }

  componentDidMount() {
    var videoPlayerHeight = this.videoPlayerRef.current.wrapper.clientHeight;
    var videoPlayerWidth = this.videoPlayerRef.current.wrapper.clientWidth;
    this.setState({ videoPlayerHeight: videoPlayerHeight, videoPlayerWidth: videoPlayerWidth });
  }

  renderVideo() {
    var boundingBoxes;
    if (this.state.currentFramePrediction && this.state.currentFramePrediction.predictionObjects) {
      boundingBoxes = this.state.currentFramePrediction.predictionObjects.map(p => {  
        var labelStyle = {
          position: 'absolute',
          fontSize: '0.5rem',
          color: 'white',
          backgroundColor: this.boundingBoxColor,
          top: ((p.boundingBox.top * this.state.videoPlayerHeight))-10,
          left: p.boundingBox.left * this.state.videoPlayerWidth,
          width: p.boundingBox.width * this.state.videoPlayerWidth,
          height: 10
        }
        
        var boundingBoxStyle = {
          position: 'absolute',
          background: 'rgba(255, 255, 255, 0)',
          border: this.boundingBoxBorderStyle,
          top: (p.boundingBox.top * this.state.videoPlayerHeight),
          left: p.boundingBox.left * this.state.videoPlayerWidth,
          width: p.boundingBox.width * this.state.videoPlayerWidth,
          height: p.boundingBox.height * this.state.videoPlayerHeight
        };

        return <div><p style={labelStyle}>{`${p.label} (${p.confidence.toFixed(2)})`}</p><div style={boundingBoxStyle}></div></div>
      });
    }
    
    return (
      <div className="video-container">
          <ReactPlayer 
            ref={this.videoPlayerRef} 
            url={this.state.uploadedVideoPath} 
            playing={this.state.playing} 
            loop={true} 
            muted={true} 
          />
          {boundingBoxes}
          <button onClick={this.startVideo}>Play</button>
          <button onClick={this.stopVideo}>Stop</button>
        </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderVideo();

    return (
      <div>
        <form onSubmit={e => this.submit(e)}>
          <div className="form-group">
            <label for='videoFile'>
              Upload video file: 
            </label>            
            <input 
              id='videoFile' 
              name='videoFile' 
              type='file' 
              className='form-control-file' 
              onChange={e => this.setFile(e)} />
          </div>
          <button type='submit' className='btn btn-primary'>Upload</button>
        </form>
        {contents}
      </div>
    );
  }

  setFile(e) {
    this.setState({ file: e.target.files[0] });
  }

  async submit(e) {
    e.preventDefault();
    this.setState({ loading: true });

    const formData = new FormData();
    formData.append('file', this.state.file);
    formData.append('projectName', this.props.customVisionProjectName);
    
    const response = await fetch('customvisionprediction/uploadvideo', {
      method: 'POST',
      headers: {
        'Accept': 'application/json'
      },
      body: formData
    });

    const data = await response.json();

    this.setState({ uploadedVideoPath: data.videoFilePath, predictions: data.predictions });
    this.setState({ loading: false });
  }

  startVideo() {
    this.setState({ playing: true });

    this.setIntervalId = setInterval(() => {
      var currentFrame = this.state.predictions.find(f => f.millisecond === this.state.currentFramePredictionIndex);
      if (currentFrame) {
        this.setState({ currentFramePrediction: currentFrame });
      }
      this.setState({ currentFramePredictionIndex: (this.state.currentFramePredictionIndex+150) })
    }, 150);
  }

  stopVideo() {
    this.setState({ playing: false });
    clearInterval(this.setIntervalId);
  }
}
