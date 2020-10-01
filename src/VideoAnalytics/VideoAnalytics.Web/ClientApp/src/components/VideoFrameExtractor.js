import React, { Component } from 'react';
import Gallery from 'react-grid-gallery';

import './VideoFrameExtractor.css';

export class VideoFrameExtractor extends Component {
  static displayName = VideoFrameExtractor.name;

  constructor(props) {
    super(props);
    this.state = { frames: [], loading: true };

    this.renderImageFrames = this.renderImageFrames.bind(this);
    this.populateImageFrames = this.populateImageFrames.bind(this);
    this.onSelectImage = this.onSelectImage.bind(this);
    this.uploadSelectedImages = this.uploadSelectedImages.bind(this);
  }

  componentDidMount() {
    this.populateImageFrames();
  }

  renderImageFrames(frames) {
    return (
      <div className='frame-list-container'>
        <Gallery images={frames} onSelectImage={this.onSelectImage} enableLightbox={false}/>
      </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderImageFrames(this.state.frames);

    return (
      <div className='row'>
        <h1 id='tabelLabel' >Train</h1>
        <div className='container'>
          <div className='row'>
            {contents}
          </div>
          <div className='row'>
            <button type='button' className='btn btn-primary' onClick={this.uploadSelectedImages}>Upload</button>
          </div>
        </div>
      </div>
    );
  }

  async populateImageFrames() {
    const response = await fetch('customvisionauthoring');
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

    this.setState({ frames: imageFrames, loading: false });
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
