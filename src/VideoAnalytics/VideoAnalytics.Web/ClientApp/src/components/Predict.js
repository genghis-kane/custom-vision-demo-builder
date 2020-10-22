import React, { Component } from 'react';
import ReactPlayer from 'react-player'
import './Predict.css';

export class Predict extends Component {
  static displayName = Predict.name;

  constructor(props) {
    super(props);
    this.state = { 
      file: null, 
      uploadedVideoPath: null, 
      predictions: null, 
      boundingBoxStyle: {
        position: 'absolute',
        border: '3px solid red',
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

    this.renderVideo = this.renderVideo.bind(this);
    this.startVideo = this.startVideo.bind(this);
  }

  renderVideo() {
    return (
      <div className="video-container">
        <ReactPlayer ref={this.videoPlayerRef} url={this.state.uploadedVideoPath} playing={this.state.playing} loop={true} muted={true} />
        <canvas ref={this.boundingBoxRef} style={this.state.boundingBoxStyle} className="bounding-box"></canvas>
        <button onClick={this.startVideo}>Play</button>
        <button onClick={() => this.setState({ playing: false })}>Stop</button>
      </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderVideo();

    return (
      <div>
        <h1 id="tabelLabel" >Predict</h1>
        <p>This component demonstrates predictions using the trained custom vision model.</p>

        <form onSubmit={e => this.submit(e)}>
          <h1>Upload video</h1>
          <input type="file" onChange={e => this.setFile(e)} />
          <button type="submit">Upload</button>
        </form>

        {contents}
      </div>
    );
  }

  setFile(e) {
    this.setState({ file: e.target.files[0] });
  }

  startVideo() {
    var videoPlayerHeight = this.videoPlayerRef.current.wrapper.clientHeight;
    var videoPlayerWidth = this.videoPlayerRef.current.wrapper.clientWidth;
    var boundingBox = this.boundingBoxRef.current.getContext("2d");
    boundingBox.fillStyle = 'red';
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
                border: '3px solid green',
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

  async submit(e) {
    e.preventDefault();
    this.setState({ loading: true });

    const formData = new FormData();
    formData.append('file', this.state.file);
    
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
}
