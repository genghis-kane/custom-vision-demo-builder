import React, { Component } from 'react';
import ReactPlayer from 'react-player'

import './Demo.css';

export class Demo extends Component {
  static displayName = Demo.name;

  constructor(props) {
    super(props);

    this.boundingBoxColor = this.props.boundingBoxColor;
    this.boundingBoxBorderStyle = `3px solid ${this.boundingBoxColor}`;

    this.state = { 
      predictions: [],
      currentFramePrediction: {},
      loading: false, 
      playing: false,
      videoPlayerHeight: 0,
      videoPlayerWidth: 0
    };

    this.videoPlayerRef = React.createRef();
    this.boundingBoxRef = React.createRef();

    this.populatePredictions = this.populatePredictions.bind(this);
    this.startVideo = this.startVideo.bind(this);
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
        var boundingBoxStyle = {
          position: 'absolute',
          border: this.boundingBoxBorderStyle,
          top: (p.boundingBox.top * this.state.videoPlayerHeight),
          left: p.boundingBox.left * this.state.videoPlayerWidth,
          width: p.boundingBox.width * this.state.videoPlayerWidth,
          height: p.boundingBox.height * this.state.videoPlayerHeight
        };
        // boundingBox.fillText(prediction.label, 0, 0) // let's leave this for now so I can go to bed
        return <canvas ref={this.boundingBoxRef} style={boundingBoxStyle} className="bounding-box"></canvas>
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
          <button onClick={() => this.setState({ playing: false })}>Stop</button>
        </div>
      </div>
    );
  }

  startVideo() {
    this.setState({ playing: true });

    var index = 0;
    setInterval(() => {
      var currentFrame = this.state.predictions.find(f => f.millisecond === index);
      if (currentFrame) {
        this.setState({ currentFramePrediction: currentFrame });
      }
      index+=300; //this will be a problem...
    }, 300);
  }

  async populatePredictions() {
    this.setState({ predictions: [], loading: true });

    const response = await fetch(this.props.predictionResultsUrl);
    const data = await response.json();
    
    this.setState({ predictions: data, loading: false });
  }
}
