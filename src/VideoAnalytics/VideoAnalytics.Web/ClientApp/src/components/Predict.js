import React, { Component } from 'react';
import { post } from 'axios';

export class Predict extends Component {
  static displayName = Predict.name;

  constructor(props) {
    super(props);
    this.state = { file: null, loading: false };

    this.renderVideo = this.renderVideo.bind(this);
  }

  renderVideo() {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
      </table>
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

  async submit(e) {
    e.preventDefault();

    const url = 'customvisionprediction/uploadvideo';
    const formData = new FormData();
    formData.append('file', this.state.file);
    const config = {
      headers: {
        'content-type': 'multipart/form-data',
      },
    };
    return post(url, formData, config);
  }
}
