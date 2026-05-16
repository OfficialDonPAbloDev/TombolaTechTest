import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/coffee_bean_logo_all_the_beans.svg'
import BeanList from './components/BeanList/BeanList'
import BeanDetail from './components/BeanDetail/BeanDetail'
import ShoppingBasket from './components/ShoppingBasket/ShoppingBasket'
import { useBasket } from './hooks/useBasket'
import './App.css'

function App() {
  const [selectedBean, setSelectedBean] = useState(null)
  const basket = useBasket()

  const handleBeanSelect = (bean) => {
    setSelectedBean(bean)
  }

  const handleCloseDetail = () => {
    setSelectedBean(null)
  }

  const handleAddToCart = (bean, quantity) => {
    basket.addItem(bean, quantity)
  }

  const handleCheckout = () => {
    const formatted = new Intl.NumberFormat(undefined, {
      style: 'currency',
      currency: basket.currency,
    }).format(basket.total)
    alert(`Checkout: ${basket.itemCount} items, total ${formatted}`)
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
          <BeanList onBeanSelect={handleBeanSelect} />
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

      <section id="basket">
        <ShoppingBasket
          items={basket.items}
          total={basket.total}
          itemCount={basket.itemCount}
          currency={basket.currency}
          onUpdateQuantity={basket.updateQuantity}
          onRemoveItem={basket.removeItem}
          onCheckout={handleCheckout}
        />
      </section>

      <BeanDetail
        bean={selectedBean}
        onClose={handleCloseDetail}
        onAddToCart={handleAddToCart}
      />

      <section id="spacer"></section>
    </>
  )
}

export default App
