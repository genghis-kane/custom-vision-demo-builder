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
      boundingBoxStyle: {
        position: 'absolute',
        border: this.boundingBoxBorderStyle,
        top: 0,
        left: 0,
        width: 0,
        height: 0
      },
      loading: false, 
      playing: false 
    };

    this.videoPlayerRef = React.createRef();
    this.boundingBoxRef = React.createRef();

    this.populatePredictions = this.populatePredictions.bind(this);
    this.startVideo = this.startVideo.bind(this);
  }

  componentDidMount() {
    this.populatePredictions();
  }

  render() {
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
          <canvas ref={this.boundingBoxRef} style={this.state.boundingBoxStyle} className="bounding-box"></canvas>
          <button onClick={this.startVideo}>Play</button>
          <button onClick={() => this.setState({ playing: false })}>Stop</button>
        </div>
      </div>
    );
  }

  startVideo() {
    var videoPlayerHeight = this.videoPlayerRef.current.wrapper.clientHeight;
    var videoPlayerWidth = this.videoPlayerRef.current.wrapper.clientWidth;
    var boundingBox = this.boundingBoxRef.current.getContext("2d");
    boundingBox.fillStyle = this.boundingBoxColor;
    boundingBox.font = '18px Arial';
    boundingBox.textAlign = 'start';
    boundingBox.textBaseline = 'start';

    this.setState({ playing: true });

    var index = 0;
    setInterval(() => {
      var currentFrame = this.state.predictions.find(f => f.millisecond === index);

      if (currentFrame) {
        var predictionObjects = currentFrame.predictionObjects;
        
        if (predictionObjects && predictionObjects.length > 0) {
          var prediction = predictionObjects[0]; // we'll just start with one for now
          
          if (prediction) {
            this.setState({
              boundingBoxStyle: {
                position: 'absolute',
                border: this.boundingBoxBorderStyle,
                top: (prediction.boundingBox.top * videoPlayerHeight),
                left: prediction.boundingBox.left * videoPlayerWidth,
                width: prediction.boundingBox.width * videoPlayerWidth,
                height: prediction.boundingBox.height * videoPlayerHeight
              }
            });

            // boundingBox.fillText(prediction.label, 0, 0) // let's leave this for now so I can go to bed
          }
        } 
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
