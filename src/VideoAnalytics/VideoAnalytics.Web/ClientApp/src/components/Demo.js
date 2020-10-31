import React, { Component } from 'react';
import ReactPlayer from 'react-player'

import './Demo.css';

export class Demo extends Component {
  static displayName = Demo.name;

  constructor(props) {
    super(props);

    this.setIntervalId = 0;

    this.boundingBoxColor = this.props.boundingBoxColor;
    this.boundingBoxBorderStyle = `3px solid ${this.boundingBoxColor}`;

    this.state = { 
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

    this.populatePredictions = this.populatePredictions.bind(this);
    this.startVideo = this.startVideo.bind(this);
    this.stopVideo = this.stopVideo.bind(this);
  }

  componentDidMount() {
    this.populatePredictions();

    var videoPlayerHeight = this.videoPlayerRef.current.wrapper.clientHeight;
    var videoPlayerWidth = this.videoPlayerRef.current.wrapper.clientWidth;
    this.setState({ videoPlayerHeight: videoPlayerHeight, videoPlayerWidth: videoPlayerWidth });
  }

  render() {
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
      <div className="demo-container">
        <div className="video-container">
          <ReactPlayer 
            ref={this.videoPlayerRef} 
            url={this.props.videoUrl} 
            playing={this.state.playing} 
            loop={true} 
            muted={true} 
          />
          {boundingBoxes}
          <button onClick={this.startVideo}>Play</button>
          <button onClick={this.stopVideo}>Stop</button>
        </div>
      </div>
    );
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

  async populatePredictions() {
    this.setState({ predictions: [], loading: true });

    const response = await fetch(this.props.predictionResultsUrl);
    const data = await response.json();
    
    this.setState({ predictions: data, loading: false });
  }
}
