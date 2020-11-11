import React, { Component } from 'react';
import CreatableSelect from 'react-select/creatable';

export class CustomVisionProjectSelector extends Component {
  static displayName = CustomVisionProjectSelector.name;

  constructor(props) {
    super(props);
    this.state = { 
      existingProjects: [],
      selectedProject: '',
      loading: false
    };

    this.getCustomVisionProjects = this.getCustomVisionProjects.bind(this);
  }

  componentDidMount() {
    this.getCustomVisionProjects();
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
              <label for='projectName'>Project name: </label>
                <CreatableSelect
                  id='projectName'
                  name='projectName'
                  isClearable
                  value={this.state.selectedProject}
                  onChange={this.handleProjectSelect}
                  onInputChange={this.handleProjectCreate}
                  options={this.state.existingProjects}
                />
              </div>
              <button type='submit' className='btn btn-primary'>Submit</button>
            </form>
          </div>
        </div> 
    );
  }

  async getCustomVisionProjects() {
    const response = await fetch('customvisionauthoring/listcvprojects');
    const data = await response.json();

    const cvProjects = data.map((p) => {
      return {
        value: p,
        label: p
      }
    });

    this.setState({ existingProjects: cvProjects });
  }

  handleProjectSelect = selectedProject => {
    this.setState({ selectedProject });
  };

  async submit(e) {
    e.preventDefault();
    this.setState({ loading: true });

    var selectedProjectName = this.state.selectedProject.value;
    const requestBody = { projectName: selectedProjectName };

    const response = await fetch('customvisionauthoring/createcvproject', {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(requestBody)
    });

    await response.json();
    this.setState({ loading: false });

    this.props.callback(selectedProjectName);
  }
}
