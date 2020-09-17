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
      <div>
        <h1 id="tabelLabel" >Train</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
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

  /* Gallery methods */
  onSelectImage (index, image) {
    var images = this.state.frames.slice();
    var img = images[index];
    if(img.hasOwnProperty("isSelected"))
        img.isSelected = !img.isSelected;
    else
        img.isSelected = true;

    this.setState({
        images: images
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
