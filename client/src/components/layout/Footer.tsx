import { Link } from 'react-router-dom';
import './Footer.css';

export const Footer = () => {
    return (
        <footer className="footer-container">
            <div className="footer-content">
                <div className="footer-brand">
                    <Link to="/" className="footer-logo">
                        Room<span className="text-primary">Connect</span>
                    </Link>
                    <p className="footer-description">
                        Discover your perfect space with RoomConnect. We make finding and renting rooms easy, secure, and hassle-free anywhere in the world.
                    </p>
                    <div className="social-links">
                        <a href="#" className="social-icon" aria-label="Facebook">
                            <span className="icon">f</span>
                        </a>
                        <a href="#" className="social-icon" aria-label="Twitter">
                            <span className="icon">t</span>
                        </a>
                        <a href="#" className="social-icon" aria-label="Instagram">
                            <span className="icon">i</span>
                        </a>
                        <a href="#" className="social-icon" aria-label="LinkedIn">
                            <span className="icon">in</span>
                        </a>
                    </div>
                </div>

                <div className="footer-links-group">
                    <div className="footer-links">
                        <h4 className="footer-heading">Quick Links</h4>
                        <ul>
                            <li><Link to="/">Home</Link></li>
                            <li><Link to="/explore">Explore Rooms</Link></li>
                            <li><Link to="/how-it-works">How it Works</Link></li>
                            <li><Link to="/pricing">Pricing</Link></li>
                        </ul>
                    </div>

                    <div className="footer-links">
                        <h4 className="footer-heading">Support</h4>
                        <ul>
                            <li><Link to="/help">Help Center</Link></li>
                            <li><Link to="/contact">Contact Us</Link></li>
                            <li><Link to="/privacy">Privacy Policy</Link></li>
                            <li><Link to="/terms">Terms of Service</Link></li>
                        </ul>
                    </div>

                    <div className="footer-newsletter">
                        <h4 className="footer-heading">Newsletter</h4>
                        <p>Subscribe to get the latest room updates and offers.</p>
                        <form className="newsletter-form" onSubmit={(e) => e.preventDefault()}>
                            <input
                                type="email"
                                placeholder="Your email address"
                                className="newsletter-input"
                                required
                            />
                            <button type="submit" className="newsletter-btn">Subscribe</button>
                        </form>
                    </div>
                </div>
            </div>

            <div className="footer-bottom">
                <p>&copy; {new Date().getFullYear()} RoomConnect. all rights reserved o Ganesh BK @Developer</p>
            </div>
        </footer>
    );
};
