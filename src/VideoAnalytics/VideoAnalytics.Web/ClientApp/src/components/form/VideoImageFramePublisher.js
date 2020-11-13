import React, { Component } from 'react';
import Gallery from 'react-grid-gallery';

import './VideoImageFramePublisher.css';

export class VideoImageFramePublisher extends Component {
  static displayName = VideoImageFramePublisher.name;

  constructor(props) {
    super(props);
    this.state = { 
      images: [],
      loading: false,
    };

    this.onSelectImage = this.onSelectImage.bind(this);
    this.uploadSelectedImages = this.uploadSelectedImages.bind(this);
  }

  componentDidUpdate(prevProps) {
    if (this.props.images !== prevProps.images) {
      this.setState({ images: this.props.images });
    } 
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
                
                <form onSubmit={this.uploadSelectedImages}>
                  <div className="form-group">
                      <div className='image-gallery-container'>
                          <Gallery images={this.state.images} onSelectImage={this.onSelectImage} enableLightbox={false}/>
                      </div>
                  </div>
                  <button type='submit' className='btn btn-primary'>Publish</button>
                </form>
            </div>
      </div>
    );
  }

  async uploadSelectedImages() {
    this.setState({ loading: true });

    var images = this.state.images;
    var requestBody = {
      frames: images.filter(i => i.isSelected).map(i => i.src),
      projectName: this.props.customVisionProjectName
    };
    
    await fetch('customvisionauthoring/uploadvideoframes', {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(requestBody)
    });

    this.setState({ loading: false });
  }

  /* Gallery methods */
  onSelectImage (index, image) {
    var images = this.state.images.slice();
    var img = images[index];
    if(img.hasOwnProperty("isSelected"))
        img.isSelected = !img.isSelected;
    else
        img.isSelected = true;

    this.setState({ images: images });

    if(this.allImagesSelected(images)){
        this.setState({ selectAllChecked: true });
    }
    else {
        this.setState({ selectAllChecked: false });
    }
  }

  allImagesSelected (images){
    var f = images.filter(
        function (img) {
            return img.isSelected === true;
        }
    );
    return f.length === images.length;
  }
}
