import { Component } from 'react';
import React from 'react';

class Clock extends Component {
  state = {
      time: ''
  }

  componentDidMount() {
      setInterval(() => {
          this.setState({ time: this.getCurrentTime() })
      }, 1000)
  }

  padZero(value) {
      return value < 10 ? '0' + value : value
  }
  
  getCurrentTime() {
      const { format = '24', timezone = '+0' } = this.props
      const date = new Date()
      const utc = date.getTime() + (date.getTimezoneOffset() * 60000)
      const localTime = new Date(utc + 3600000 * parseInt(timezone, 10))

      let hours = localTime.getHours()
      let minutes = localTime.getMinutes()
      let seconds = localTime.getSeconds()

      if (format === '12') {
          hours = hours % 12
          return `${this.padZero(hours)}:${this.padZero(minutes)}:${this.padZero(seconds)}`
      } else {
          return `${this.padZero(hours)}:${this.padZero(minutes)}:${this.padZero(seconds)}`
      }
  }

  render() {
      return (
          <div className='clock'>
              {this.state.time}
          </div>
      )
  }
}

export default Clock;