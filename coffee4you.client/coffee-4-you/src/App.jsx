import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/coffee_bean_logo_all_the_beans.svg'
import BeanItem from './components/BeanItem'
import './App.css'

const sampleBean = {
  id: '66a37459771606d916a226ff',
  index: 3,
  isBeanOfTheDay: true,
  cost: 17.69,
  currency: 'GBP',
  imageUrl: 'https://images.unsplash.com/photo-1598198192305-46b0805890d3',
  colour: 'dark roast',
  name: 'RONBERT',
  description:
    'Et deserunt nisi in anim cillum sint voluptate proident. Est occaecat id cupidatat cupidatat ex veniam irure veniam pariatur excepteur duis labore occaecat amet. Culpa adipisicing nisi esse consequat adipisicing anim.',
  country: 'Brazil',
}

function App() {
  const [count, setCount] = useState(0)
  const handleBeanSelect = (bean) => {
    alert(`${bean.name} clicked!!`)
  }

  return (
    <>
      <section id="center">
        <div className="hero">
          <img src={heroImg} className="base" width="170" height="179" alt="" />
        </div>
        <div>
          <h1>Your world of coffee</h1>
          <p>
            Welcome to <b>All The Beans</b> — your ultimate destination for the finest coffee 
            beans from every corner of the globe. Whether you're a seasoned espresso 
            enthusiast or just beginning your coffee journey, we've curated an 
            exceptional collection of single-origin gems, expertly blended roasts, and 
            rare small-batch varieties to satisfy every palate. From Ethiopia to Colombia, every 
            bag we stock tells a story of passion, craft, and quality. Browse our full range, 
            discover tasting notes, get expert brewing advice, and have your perfect 
            beans delivered straight to your door — because great coffee starts with 
            great beans, and we have all of them just for you!
          </p>
        </div>
      </section>

      <section id="next-steps">
        <div id="docs">
          <svg className="icon" role="presentation" aria-hidden="true">
            <use href="/icons.svg#coffee-jar-icon"></use>
          </svg>
          <h2>Shop Floor</h2>
          <p>The best place to find the finest coffee beans from around the world</p>
          <BeanItem bean={sampleBean} onSelect={handleBeanSelect} />
        </div>
        <div id="social">
          <svg className="icon" role="presentation" aria-hidden="true">
            <use href="/icons.svg#social-icon"></use>
          </svg>
          <h2>Connect with us</h2>
          <p>Join the "All the beans" community</p>
          <ul>
            <li >
              <a href="https://github.com/vitejs/vite" target="_blank">
                <svg
                  className="button-icon"
                  role="presentation"
                  aria-hidden="true"
                >
                  <use href="/icons.svg#github-icon"></use>
                </svg>
                GitHub
              </a>
            </li>
            <li>
              <a href="https://chat.vite.dev/" target="_blank">
                <svg
                  className="button-icon"
                  role="presentation"
                  aria-hidden="true"
                >
                  <use href="/icons.svg#discord-icon"></use>
                </svg>
                Discord
              </a>
            </li>
            <li>
              <a href="https://x.com/vite_js" target="_blank">
                <svg
                  className="button-icon"
                  role="presentation"
                  aria-hidden="true"
                >
                  <use href="/icons.svg#x-icon"></use>
                </svg>
                X.com
              </a>
            </li>
            <li>
              <a href="https://bsky.app/profile/vite.dev" target="_blank">
                <svg
                  className="button-icon"
                  role="presentation"
                  aria-hidden="true"
                >
                  <use href="/icons.svg#bluesky-icon"></use>
                </svg>
                Bluesky
              </a>
            </li>
          </ul>
        </div>
      </section>
      <section id="spacer"></section>
    </>
  )
}

export default App
